﻿// 
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
// 
// Microsoft Bot Framework: http://botframework.com
// 
// Bot Builder SDK Github:
// https://github.com/Microsoft/BotBuilder
// 
// Copyright (c) Microsoft Corporation
// All rights reserved.
// 
// MIT License:
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Connector;

namespace Microsoft.Bot.Builder.Dialogs.Internals
{
    public interface IScorable<Score>
    {
        Task<object> PrepareAsync<Item>(Item item, CancellationToken token);
        bool TryScore(object state, out Score score);
        Task PostAsync<Item>(Item item, object state, CancellationToken token);
    }

    public sealed class CompositeScorable<Score> : IScorable<Score>
    {
        private readonly IComparer<Score> comparer;
        private readonly ITraits<Score> traits;
        private readonly IEnumerable<IScorable<Score>> scorables;
        public CompositeScorable(IComparer<Score> comparer, ITraits<Score> traits, params IScorable<Score>[] scorables)
            : this(comparer, traits, (IEnumerable<IScorable<Score>>)scorables)
        {
        }
        public CompositeScorable(IComparer<Score> comparer, ITraits<Score> traits, IEnumerable<IScorable<Score>> scorables)
        {
            SetField.NotNull(out this.comparer, nameof(comparer), comparer);
            SetField.NotNull(out this.traits, nameof(traits), traits);
            SetField.NotNull(out this.scorables, nameof(scorables), scorables);
        }

        public sealed class CompositeState
        {
            public Score Score { get; set; }
            public object State { get; set; }
            public IScorable<Score> Scorable { get; set; }
        }

        async Task<object> IScorable<Score>.PrepareAsync<Item>(Item item, CancellationToken token)
        {
            CompositeState maximum = null;

            foreach (var scorable in this.scorables)
            {
                var state = await scorable.PrepareAsync(item, token);
                Score score;
                if (scorable.TryScore(state, out score))
                {
                    if (this.comparer.Compare(score, this.traits.Minimum) < 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(score));
                    }

                    if (this.comparer.Compare(score, this.traits.Maximum) > 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(score));
                    }

                    if (maximum == null)
                    {
                        maximum = new CompositeState();
                    }

                    var compare = this.comparer.Compare(score, maximum.Score);
                    if (maximum.Scorable == null || compare > 0)
                    {
                        maximum.Score = score;
                        maximum.State = state;
                        maximum.Scorable = scorable;

                        if (this.comparer.Compare(score, this.traits.Maximum) == 0)
                        {
                            break;
                        }
                    }
                }
            }

            return maximum;
        }
        bool IScorable<Score>.TryScore(object state, out Score score)
        {
            var maximum = (CompositeState)state;
            bool matches = maximum != null;
            score = matches ? maximum.Score : default(Score);
            return matches;
        }
        async Task IScorable<Score>.PostAsync<Item>(Item item, object state, CancellationToken token)
        {
            var maximum = (CompositeState)state;
            await maximum.Scorable.PostAsync(item, maximum.State, token);
        }
    }

    public sealed class ScoringDialogTask<Score> : IPostToBot
    {
        private readonly IPostToBot inner;
        private readonly IDialogStack stack;
        private readonly IScorable<Score> scorable;
        public ScoringDialogTask(IPostToBot inner, IDialogStack stack, IScorable<Score> scorable)
        {
            SetField.NotNull(out this.inner, nameof(inner), inner);
            SetField.NotNull(out this.stack, nameof(stack), stack);
            SetField.NotNull(out this.scorable, nameof(scorable), scorable);
        }

        async Task IPostToBot.PostAsync<T>(T item, CancellationToken token)
        {
            var state = await this.scorable.PrepareAsync<T>(item, token);
            Score score;
            if (this.scorable.TryScore(state, out score))
            {
                await this.scorable.PostAsync<T>(item, state, token);
            }
            else
            {
                await this.inner.PostAsync<T>(item, token);
            }
        }
    }


    public partial class Extensions
    {
        public static IDialog<T> WithScorable<T, Score>(this IDialog<T> antecedent, IScorable<Score> scorable)
        {
            return new WithScorableDialog<T, Score>(antecedent, scorable);
        }

        [Serializable]
        private sealed class WithScorableDialog<T, Score> : IDialog<T>, IScorable<Score>
        {
            public readonly IDialog<T> Antecedent;
            public readonly IScorable<Score> Scorable;
            public WithScorableDialog(IDialog<T> antecedent, IScorable<Score> scorable)
            {
                SetField.NotNull(out this.Antecedent, nameof(antecedent), antecedent);
                SetField.NotNull(out this.Scorable, nameof(scorable), scorable);
            }
            async Task IDialog<T>.StartAsync(IDialogContext context)
            {
                context.Call<T>(this.Antecedent, ResumeAsync);
            }
            private async Task ResumeAsync(IDialogContext context, IAwaitable<T> result)
            {
                context.Done(await result);
            }
            async Task<object> IScorable<Score>.PrepareAsync<Item>(Item item, CancellationToken token)
            {
                return await this.Scorable.PrepareAsync(item, token);
            }
            bool IScorable<Score>.TryScore(object state, out Score score)
            {
                return this.Scorable.TryScore(state, out score);
            }
            async Task IScorable<Score>.PostAsync<Item>(Item item, object state, CancellationToken token)
            {
                await this.Scorable.PostAsync(item, state, token);
            }
        }
    }
}

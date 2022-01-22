using Plml;
using System;

namespace Plml
{
    public static class SpiesExtensions
    {
        public static ISpySetupPropertiesNode<TModel> When<TModel>(this ISpyConfig setup, TModel model) => setup.When(() => model);

        public static ISpyValueArrayNode<TStruct> WhenAny<TStruct>(this ISpyConfig setup, TStruct[] arr) where TStruct : struct => setup.WhenAny(() => arr);

        public static ISpyConfig Do<TModel>(this ISpySetupActionNode<TModel> setupActionNode, Action action) => setupActionNode.Do(_ => action());

        public static ISpyConfig Do<TModel>(this ISpySetupActionNode<TModel> setupAction, params Action[] actions) => setupAction.Do(actions.Combine());
    }
}
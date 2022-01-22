using Plml.Checkers;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

namespace Plml
{
    internal class SpyImpl : ISpy
    {
        private readonly ICollection<Action> detectActions = new List<Action>();

        public void DetectChanges()
        {
            foreach (Action action in detectActions)
                action.Invoke();
        }

        public ISpyValueArrayNode<TStruct> WhenAny<TStruct>(Func<TStruct[]> arraySource) where TStruct : struct => new ManySpySetup<TStruct>(this, arraySource);

        public IBindPropertiesSourceNode<TProp> Bind<TTarget, TProp>(TTarget target, Expression<Func<TTarget, TProp>> propSelector) => new BindingCollector<TProp>(this, target, propSelector);

        public ISpySetupPropertiesNode<TModel> When<TModel>(Func<TModel> model) => new SpySetupCollector<TModel>(this, model);

        public IBindPropertiesSourceNode<TProp1, TProp2> Bind<TTarget, TProp1, TProp2>(TTarget target, Expression<Func<TTarget, TProp1>> propSelector1, Expression<Func<TTarget, TProp2>> propSelector2) => new BindingCollector<TProp1, TProp2>(this, target, propSelector1, propSelector2);

        public IBindPropertiesSourceNode<TProp1, TProp2, TProp3> Bind<TTarget, TProp1, TProp2, TProp3>(TTarget target, Expression<Func<TTarget, TProp1>> propSelector1, Expression<Func<TTarget, TProp2>> propSelector2, Expression<Func<TTarget, TProp3>> propSelector3) => new BindingCollector<TProp1, TProp2, TProp3>(this, target, propSelector1, propSelector2, propSelector3);

        public IBindPropertiesSourceNode<TProp1, TProp2, TProp3, TProp4> Bind<TTarget, TProp1, TProp2, TProp3, TProp4>(TTarget target, Expression<Func<TTarget, TProp1>> propSelector1, Expression<Func<TTarget, TProp2>> propSelector2, Expression<Func<TTarget, TProp3>> propSelector3, Expression<Func<TTarget, TProp4>> propSelector4) => new BindingCollector<TProp1, TProp2, TProp3, TProp4>(this, target, propSelector1, propSelector2, propSelector3, propSelector4);

        public IBindPropertiesSourceNode<TProp1, TProp2, TProp3, TProp4, TProp5> Bind<TTarget, TProp1, TProp2, TProp3, TProp4, TProp5>(TTarget target, Expression<Func<TTarget, TProp1>> propSelector1, Expression<Func<TTarget, TProp2>> propSelector2, Expression<Func<TTarget, TProp3>> propSelector3, Expression<Func<TTarget, TProp4>> propSelector4, Expression<Func<TTarget, TProp5>> propSelector5) => new BindingCollector<TProp1, TProp2, TProp3, TProp4, TProp5>(this, target, propSelector1, propSelector2, propSelector3, propSelector4, propSelector5);

        public IBindPropertiesSourceNode<TProp1, TProp2, TProp3, TProp4, TProp5, TProp6> Bind<TTarget, TProp1, TProp2, TProp3, TProp4, TProp5, TProp6>(TTarget target, Expression<Func<TTarget, TProp1>> propSelector1, Expression<Func<TTarget, TProp2>> propSelector2, Expression<Func<TTarget, TProp3>> propSelector3, Expression<Func<TTarget, TProp4>> propSelector4, Expression<Func<TTarget, TProp5>> propSelector5, Expression<Func<TTarget, TProp6>> propSelector6) => new BindingCollector<TProp1, TProp2, TProp3, TProp4, TProp5, TProp6>(this, target, propSelector1, propSelector2, propSelector3, propSelector4, propSelector5, propSelector6);

        public IBindPropertiesSourceNode<TProp1, TProp2, TProp3, TProp4, TProp5, TProp6, TProp7> Bind<TTarget, TProp1, TProp2, TProp3, TProp4, TProp5, TProp6, TProp7>(TTarget target, Expression<Func<TTarget, TProp1>> propSelector1, Expression<Func<TTarget, TProp2>> propSelector2, Expression<Func<TTarget, TProp3>> propSelector3, Expression<Func<TTarget, TProp4>> propSelector4, Expression<Func<TTarget, TProp5>> propSelector5, Expression<Func<TTarget, TProp6>> propSelector6, Expression<Func<TTarget, TProp7>> propSelector7) => new BindingCollector<TProp1, TProp2, TProp3, TProp4, TProp5, TProp6, TProp7>(this, target, propSelector1, propSelector2, propSelector3, propSelector4, propSelector5, propSelector6, propSelector7);

        public IBindPropertiesSourceNode<TProp1, TProp2, TProp3, TProp4, TProp5, TProp6, TProp7, TProp8> Bind<TTarget, TProp1, TProp2, TProp3, TProp4, TProp5, TProp6, TProp7, TProp8>(TTarget target, Expression<Func<TTarget, TProp1>> propSelector1, Expression<Func<TTarget, TProp2>> propSelector2, Expression<Func<TTarget, TProp3>> propSelector3, Expression<Func<TTarget, TProp4>> propSelector4, Expression<Func<TTarget, TProp5>> propSelector5, Expression<Func<TTarget, TProp6>> propSelector6, Expression<Func<TTarget, TProp7>> propSelector7, Expression<Func<TTarget, TProp8>> propSelector8) => new BindingCollector<TProp1, TProp2, TProp3, TProp4, TProp5, TProp6, TProp7, TProp8>(this, target, propSelector1, propSelector2, propSelector3, propSelector4, propSelector5, propSelector6, propSelector7, propSelector8);

        private class SpySetupCollector<TModel> :
            ISpySetupActionNode<TModel>,
            ISpySetupPropertiesNode<TModel>
        {
            private readonly SpyImpl parent;

            private Func<TModel> modelProvider;
            private IChecker<TModel> checker;

            public SpySetupCollector(SpyImpl parent, Func<TModel> modelProvider)
            {
                this.parent = parent;
                this.modelProvider = modelProvider;
            }

            public ISpyConfig Do(Action<TModel> action)
            {
                Action resultingAction = () =>
                {
                    TModel model = modelProvider();
                    if (checker.CheckForChanges(model))
                        action(model);
                };

                parent.detectActions.Add(resultingAction);

                return parent;
            }

            public ISpySetupActionNode<TModel> HasChanged()
            {
                if (typeof(TModel).Implements<IEquatable<TModel>>())
                    checker = ValueCheckerFactory.For<TModel>();
                else
                    checker = CheckerFactory<TModel>.ForAllProperties();
                return this;
            }

            public ISpySetupActionNode<TModel> HasChangesOn<T>(Func<TModel, T> prop)
            {
                checker = CheckerFactory<TModel>.ForProperty(prop);
                return this;
            }

            public ISpySetupActionNode<TModel> HasChangesOn<T1, T2>(Func<TModel, T1> prop1, Func<TModel, T2> prop2)
            {
                checker = CheckerFactory<TModel>.ForProperties(prop1, prop2);
                return this;
            }

            public ISpySetupActionNode<TModel> HasChangesOn<T1, T2, T3>(Func<TModel, T1> prop1, Func<TModel, T2> prop2, Func<TModel, T3> prop3)
            {
                checker = CheckerFactory<TModel>.ForProperties(prop1, prop2, prop3);
                return this;
            }

            public ISpySetupActionNode<TModel> HasChangesOn<T1, T2, T3, T4>(Func<TModel, T1> prop1, Func<TModel, T2> prop2, Func<TModel, T3> prop3, Func<TModel, T4> prop4)
            {
                checker = CheckerFactory<TModel>.ForProperties(prop1, prop2, prop3, prop4);
                return this;
            }

            public ISpySetupActionNode<TModel> HasChangesOn<T1, T2, T3, T4, T5>(Func<TModel, T1> prop1, Func<TModel, T2> prop2, Func<TModel, T3> prop3, Func<TModel, T4> prop4, Func<TModel, T5> prop5)
            {
                checker = CheckerFactory<TModel>.ForProperties(prop1, prop2, prop3, prop4, prop5);
                return this;
            }

            public ISpySetupActionNode<TModel> HasChangesOn<T1, T2, T3, T4, T5, T6>(Func<TModel, T1> prop1, Func<TModel, T2> prop2, Func<TModel, T3> prop3, Func<TModel, T4> prop4, Func<TModel, T5> prop5, Func<TModel, T6> prop6)
            {
                checker = CheckerFactory<TModel>.ForProperties(prop1, prop2, prop3, prop4, prop5, prop6);
                return this;
            }

            public ISpySetupActionNode<TModel> HasChangesOn<T1, T2, T3, T4, T5, T6, T7>(Func<TModel, T1> prop1, Func<TModel, T2> prop2, Func<TModel, T3> prop3, Func<TModel, T4> prop4, Func<TModel, T5> prop5, Func<TModel, T6> prop6, Func<TModel, T7> prop7)
            {
                checker = CheckerFactory<TModel>.ForProperties(prop1, prop2, prop3, prop4, prop5, prop6, prop7);
                return this;
            }

            public ISpySetupActionNode<TModel> HasChangesOn<T1, T2, T3, T4, T5, T6, T7, T8>(Func<TModel, T1> prop1, Func<TModel, T2> prop2, Func<TModel, T3> prop3, Func<TModel, T4> prop4, Func<TModel, T5> prop5, Func<TModel, T6> prop6, Func<TModel, T7> prop7, Func<TModel, T8> prop8)
            {
                checker = CheckerFactory<TModel>.ForProperties(prop1, prop2, prop3, prop4, prop5, prop6, prop7, prop8);
                return this;
            }
        }

        private class ManySpySetup<TStruct> :
            ISpyValueArrayNode<TStruct>,
            ISpySetupActionNode<TStruct[]>
            where TStruct : struct

        {
            private readonly SpyImpl parent;
            private readonly Func<TStruct[]> arrayProvider;

            public ManySpySetup(SpyImpl parent, Func<TStruct[]> arrayProvider)
            {
                this.parent = parent;
                this.arrayProvider = arrayProvider;
            }

            public ISpyConfig Do(Action<TStruct[]> action)
            {
                IChecker<TStruct[]> checker = CheckerFactory.ForValueArray<TStruct>();

                Action resultingAction = () =>
                {
                    TStruct[] array = arrayProvider();

                    if (checker.CheckForChanges(array))
                        action(array);
                };

                parent.detectActions.Add(resultingAction);

                return parent;
            }

            public ISpySetupActionNode<TStruct[]> HasChanged() => this;
        }

        #region BindingCollection
        private abstract class BindingCollector
        {
            protected readonly SpyImpl parent;
            protected readonly object target;

            protected BindingCollector(SpyImpl parent, object target)
            {
                this.parent = parent;
                this.target = target;
            }

            protected Action CreateBinding(object targetObj, LambdaExpression targetPropAccessor, object sourceObj, LambdaExpression sourcePropAccessor)
            {
                MemberExpression targetME = (MemberExpression)targetPropAccessor.Body;
                PropertyInfo targetPI = (PropertyInfo)targetME.Member;

                MemberExpression sourceME = (MemberExpression)sourcePropAccessor.Body;
                PropertyInfo sourcePI = (PropertyInfo)sourceME.Member;

                Expression<Action> lambda = Expression.Lambda<Action>(Expression.Assign(
                    Expression.Property(Expression.Constant(targetObj), targetPI),
                    Expression.Property(Expression.Constant(sourceObj), sourcePI)
                ));

                return lambda.Compile();
            }
        }

        private class BindingCollector<TProp> : BindingCollector, IBindPropertiesSourceNode<TProp>
        {
            private readonly LambdaExpression targetPropAccessor;

            public BindingCollector(SpyImpl parent, object target, LambdaExpression targetPropAccessor) : base(parent, target)
            {
                this.targetPropAccessor = targetPropAccessor;
            }

            public ISpyConfig To<TSource>(TSource source, Expression<Func<TSource, TProp>> sourcePropAccessor)
            {
                Action binding = CreateBinding(target, targetPropAccessor, source, sourcePropAccessor);

                parent.detectActions.Add(binding);
                return parent;
            }
        }

        private class BindingCollector<TProp1, TProp2> : BindingCollector, IBindPropertiesSourceNode<TProp1, TProp2>
        {
            private readonly LambdaExpression targetPropAccessor1;
            private readonly LambdaExpression targetPropAccessor2;

            public BindingCollector(
                SpyImpl parent,
                object target,
                LambdaExpression targetPropAccessor1,
                LambdaExpression targetPropAccessor2)
                : base(parent, target)
            {
                this.targetPropAccessor1 = targetPropAccessor1;
                this.targetPropAccessor2 = targetPropAccessor2;
            }

            public ISpyConfig To<TSource>(
                TSource source,
                Expression<Func<TSource, TProp1>> sourcePropAccessor1,
                Expression<Func<TSource, TProp2>> sourcePropAccessor2)
            {
                Action binding1 = CreateBinding(target, targetPropAccessor1, source, sourcePropAccessor1);
                Action binding2 = CreateBinding(target, targetPropAccessor2, source, sourcePropAccessor2);

                parent.detectActions.Add(binding1);
                parent.detectActions.Add(binding2);

                return parent;
            }
        }

        private class BindingCollector<TProp1, TProp2, TProp3> : BindingCollector, IBindPropertiesSourceNode<TProp1, TProp2, TProp3>
        {
            private readonly LambdaExpression targetPropAccessor1;
            private readonly LambdaExpression targetPropAccessor2;
            private readonly LambdaExpression targetPropAccessor3;

            public BindingCollector(
                SpyImpl parent,
                object target,
                LambdaExpression targetPropAccessor1,
                LambdaExpression targetPropAccessor2,
                LambdaExpression targetPropAccessor3)
                : base(parent, target)
            {
                this.targetPropAccessor1 = targetPropAccessor1;
                this.targetPropAccessor2 = targetPropAccessor2;
                this.targetPropAccessor3 = targetPropAccessor3;
            }

            public ISpyConfig To<TSource>(
                TSource source,
                Expression<Func<TSource, TProp1>> sourcePropAccessor1,
                Expression<Func<TSource, TProp2>> sourcePropAccessor2,
                Expression<Func<TSource, TProp3>> sourcePropAccessor3)
            {
                Action binding1 = CreateBinding(target, targetPropAccessor1, source, sourcePropAccessor1);
                Action binding2 = CreateBinding(target, targetPropAccessor2, source, sourcePropAccessor2);
                Action binding3 = CreateBinding(target, targetPropAccessor3, source, sourcePropAccessor3);

                parent.detectActions.Add(binding1);
                parent.detectActions.Add(binding2);
                parent.detectActions.Add(binding3);

                return parent;
            }
        }

        private class BindingCollector<TProp1, TProp2, TProp3, TProp4> : BindingCollector, IBindPropertiesSourceNode<TProp1, TProp2, TProp3, TProp4>
        {
            private readonly LambdaExpression targetPropAccessor1;
            private readonly LambdaExpression targetPropAccessor2;
            private readonly LambdaExpression targetPropAccessor3;
            private readonly LambdaExpression targetPropAccessor4;

            public BindingCollector(
                SpyImpl parent,
                object target,
                LambdaExpression targetPropAccessor1,
                LambdaExpression targetPropAccessor2,
                LambdaExpression targetPropAccessor3,
                LambdaExpression targetPropAccessor4)
                : base(parent, target)
            {
                this.targetPropAccessor1 = targetPropAccessor1;
                this.targetPropAccessor2 = targetPropAccessor2;
                this.targetPropAccessor3 = targetPropAccessor3;
                this.targetPropAccessor4 = targetPropAccessor4;
            }

            public ISpyConfig To<TSource>(
                TSource source,
                Expression<Func<TSource, TProp1>> sourcePropAccessor1,
                Expression<Func<TSource, TProp2>> sourcePropAccessor2,
                Expression<Func<TSource, TProp3>> sourcePropAccessor3,
                Expression<Func<TSource, TProp4>> sourcePropAccessor4)
            {
                Action binding1 = CreateBinding(target, targetPropAccessor1, source, sourcePropAccessor1);
                Action binding2 = CreateBinding(target, targetPropAccessor2, source, sourcePropAccessor2);
                Action binding3 = CreateBinding(target, targetPropAccessor3, source, sourcePropAccessor3);
                Action binding4 = CreateBinding(target, targetPropAccessor4, source, sourcePropAccessor4);

                parent.detectActions.Add(binding1);
                parent.detectActions.Add(binding2);
                parent.detectActions.Add(binding3);
                parent.detectActions.Add(binding4);

                return parent;
            }
        }

        private class BindingCollector<TProp1, TProp2, TProp3, TProp4, TProp5> : BindingCollector, IBindPropertiesSourceNode<TProp1, TProp2, TProp3, TProp4, TProp5>
        {
            private readonly LambdaExpression targetPropAccessor1;
            private readonly LambdaExpression targetPropAccessor2;
            private readonly LambdaExpression targetPropAccessor3;
            private readonly LambdaExpression targetPropAccessor4;
            private readonly LambdaExpression targetPropAccessor5;

            public BindingCollector(
                SpyImpl parent,
                object target,
                LambdaExpression targetPropAccessor1,
                LambdaExpression targetPropAccessor2,
                LambdaExpression targetPropAccessor3,
                LambdaExpression targetPropAccessor4,
                LambdaExpression targetPropAccessor5)
                : base(parent, target)
            {
                this.targetPropAccessor1 = targetPropAccessor1;
                this.targetPropAccessor2 = targetPropAccessor2;
                this.targetPropAccessor3 = targetPropAccessor3;
                this.targetPropAccessor4 = targetPropAccessor4;
                this.targetPropAccessor5 = targetPropAccessor5;
            }

            public ISpyConfig To<TSource>(
                TSource source,
                Expression<Func<TSource, TProp1>> sourcePropAccessor1,
                Expression<Func<TSource, TProp2>> sourcePropAccessor2,
                Expression<Func<TSource, TProp3>> sourcePropAccessor3,
                Expression<Func<TSource, TProp4>> sourcePropAccessor4,
                Expression<Func<TSource, TProp5>> sourcePropAccessor5)
            {
                Action binding1 = CreateBinding(target, targetPropAccessor1, source, sourcePropAccessor1);
                Action binding2 = CreateBinding(target, targetPropAccessor2, source, sourcePropAccessor2);
                Action binding3 = CreateBinding(target, targetPropAccessor3, source, sourcePropAccessor3);
                Action binding4 = CreateBinding(target, targetPropAccessor4, source, sourcePropAccessor4);
                Action binding5 = CreateBinding(target, targetPropAccessor5, source, sourcePropAccessor5);

                parent.detectActions.Add(binding1);
                parent.detectActions.Add(binding2);
                parent.detectActions.Add(binding3);
                parent.detectActions.Add(binding4);
                parent.detectActions.Add(binding5);

                return parent;
            }
        }

        private class BindingCollector<TProp1, TProp2, TProp3, TProp4, TProp5, TProp6> : BindingCollector, IBindPropertiesSourceNode<TProp1, TProp2, TProp3, TProp4, TProp5, TProp6>
        {
            private readonly LambdaExpression targetPropAccessor1;
            private readonly LambdaExpression targetPropAccessor2;
            private readonly LambdaExpression targetPropAccessor3;
            private readonly LambdaExpression targetPropAccessor4;
            private readonly LambdaExpression targetPropAccessor5;
            private readonly LambdaExpression targetPropAccessor6;

            public BindingCollector(
                SpyImpl parent,
                object target,
                LambdaExpression targetPropAccessor1,
                LambdaExpression targetPropAccessor2,
                LambdaExpression targetPropAccessor3,
                LambdaExpression targetPropAccessor4,
                LambdaExpression targetPropAccessor5,
                LambdaExpression targetPropAccessor6)
                : base(parent, target)
            {
                this.targetPropAccessor1 = targetPropAccessor1;
                this.targetPropAccessor2 = targetPropAccessor2;
                this.targetPropAccessor3 = targetPropAccessor3;
                this.targetPropAccessor4 = targetPropAccessor4;
                this.targetPropAccessor5 = targetPropAccessor5;
                this.targetPropAccessor6 = targetPropAccessor6;
            }

            public ISpyConfig To<TSource>(
                TSource source,
                Expression<Func<TSource, TProp1>> sourcePropAccessor1,
                Expression<Func<TSource, TProp2>> sourcePropAccessor2,
                Expression<Func<TSource, TProp3>> sourcePropAccessor3,
                Expression<Func<TSource, TProp4>> sourcePropAccessor4,
                Expression<Func<TSource, TProp5>> sourcePropAccessor5,
                Expression<Func<TSource, TProp6>> sourcePropAccessor6)
            {
                Action binding1 = CreateBinding(target, targetPropAccessor1, source, sourcePropAccessor1);
                Action binding2 = CreateBinding(target, targetPropAccessor2, source, sourcePropAccessor2);
                Action binding3 = CreateBinding(target, targetPropAccessor3, source, sourcePropAccessor3);
                Action binding4 = CreateBinding(target, targetPropAccessor4, source, sourcePropAccessor4);
                Action binding5 = CreateBinding(target, targetPropAccessor5, source, sourcePropAccessor5);
                Action binding6 = CreateBinding(target, targetPropAccessor6, source, sourcePropAccessor6);

                parent.detectActions.Add(binding1);
                parent.detectActions.Add(binding2);
                parent.detectActions.Add(binding3);
                parent.detectActions.Add(binding4);
                parent.detectActions.Add(binding5);
                parent.detectActions.Add(binding6);

                return parent;
            }
        }

        private class BindingCollector<TProp1, TProp2, TProp3, TProp4, TProp5, TProp6, TProp7> : BindingCollector, IBindPropertiesSourceNode<TProp1, TProp2, TProp3, TProp4, TProp5, TProp6, TProp7>
        {
            private readonly LambdaExpression targetPropAccessor1;
            private readonly LambdaExpression targetPropAccessor2;
            private readonly LambdaExpression targetPropAccessor3;
            private readonly LambdaExpression targetPropAccessor4;
            private readonly LambdaExpression targetPropAccessor5;
            private readonly LambdaExpression targetPropAccessor6;
            private readonly LambdaExpression targetPropAccessor7;

            public BindingCollector(
                SpyImpl parent,
                object target,
                LambdaExpression targetPropAccessor1,
                LambdaExpression targetPropAccessor2,
                LambdaExpression targetPropAccessor3,
                LambdaExpression targetPropAccessor4,
                LambdaExpression targetPropAccessor5,
                LambdaExpression targetPropAccessor6,
                LambdaExpression targetPropAccessor7)
                : base(parent, target)
            {
                this.targetPropAccessor1 = targetPropAccessor1;
                this.targetPropAccessor2 = targetPropAccessor2;
                this.targetPropAccessor3 = targetPropAccessor3;
                this.targetPropAccessor4 = targetPropAccessor4;
                this.targetPropAccessor5 = targetPropAccessor5;
                this.targetPropAccessor6 = targetPropAccessor6;
                this.targetPropAccessor7 = targetPropAccessor7;
            }

            public ISpyConfig To<TSource>(
                TSource source,
                Expression<Func<TSource, TProp1>> sourcePropAccessor1,
                Expression<Func<TSource, TProp2>> sourcePropAccessor2,
                Expression<Func<TSource, TProp3>> sourcePropAccessor3,
                Expression<Func<TSource, TProp4>> sourcePropAccessor4,
                Expression<Func<TSource, TProp5>> sourcePropAccessor5,
                Expression<Func<TSource, TProp6>> sourcePropAccessor6,
                Expression<Func<TSource, TProp7>> sourcePropAccessor7)
            {
                Action binding1 = CreateBinding(target, targetPropAccessor1, source, sourcePropAccessor1);
                Action binding2 = CreateBinding(target, targetPropAccessor2, source, sourcePropAccessor2);
                Action binding3 = CreateBinding(target, targetPropAccessor3, source, sourcePropAccessor3);
                Action binding4 = CreateBinding(target, targetPropAccessor4, source, sourcePropAccessor4);
                Action binding5 = CreateBinding(target, targetPropAccessor5, source, sourcePropAccessor5);
                Action binding6 = CreateBinding(target, targetPropAccessor6, source, sourcePropAccessor6);
                Action binding7 = CreateBinding(target, targetPropAccessor7, source, sourcePropAccessor7);

                parent.detectActions.Add(binding1);
                parent.detectActions.Add(binding2);
                parent.detectActions.Add(binding3);
                parent.detectActions.Add(binding4);
                parent.detectActions.Add(binding5);
                parent.detectActions.Add(binding6);
                parent.detectActions.Add(binding7);

                return parent;
            }
        }

        private class BindingCollector<TProp1, TProp2, TProp3, TProp4, TProp5, TProp6, TProp7, TProp8> : BindingCollector, IBindPropertiesSourceNode<TProp1, TProp2, TProp3, TProp4, TProp5, TProp6, TProp7, TProp8>
        {
            private readonly LambdaExpression targetPropAccessor1;
            private readonly LambdaExpression targetPropAccessor2;
            private readonly LambdaExpression targetPropAccessor3;
            private readonly LambdaExpression targetPropAccessor4;
            private readonly LambdaExpression targetPropAccessor5;
            private readonly LambdaExpression targetPropAccessor6;
            private readonly LambdaExpression targetPropAccessor7;
            private readonly LambdaExpression targetPropAccessor8;

            public BindingCollector(
                SpyImpl parent,
                object target,
                LambdaExpression targetPropAccessor1,
                LambdaExpression targetPropAccessor2,
                LambdaExpression targetPropAccessor3,
                LambdaExpression targetPropAccessor4,
                LambdaExpression targetPropAccessor5,
                LambdaExpression targetPropAccessor6,
                LambdaExpression targetPropAccessor7,
                LambdaExpression targetPropAccessor8)
                : base(parent, target)
            {
                this.targetPropAccessor1 = targetPropAccessor1;
                this.targetPropAccessor2 = targetPropAccessor2;
                this.targetPropAccessor3 = targetPropAccessor3;
                this.targetPropAccessor4 = targetPropAccessor4;
                this.targetPropAccessor5 = targetPropAccessor5;
                this.targetPropAccessor6 = targetPropAccessor6;
                this.targetPropAccessor7 = targetPropAccessor7;
                this.targetPropAccessor8 = targetPropAccessor8;
            }

            public ISpyConfig To<TSource>(
                TSource source,
                Expression<Func<TSource, TProp1>> sourcePropAccessor1,
                Expression<Func<TSource, TProp2>> sourcePropAccessor2,
                Expression<Func<TSource, TProp3>> sourcePropAccessor3,
                Expression<Func<TSource, TProp4>> sourcePropAccessor4,
                Expression<Func<TSource, TProp5>> sourcePropAccessor5,
                Expression<Func<TSource, TProp6>> sourcePropAccessor6,
                Expression<Func<TSource, TProp7>> sourcePropAccessor7,
                Expression<Func<TSource, TProp8>> sourcePropAccessor8)
            {
                Action binding1 = CreateBinding(target, targetPropAccessor1, source, sourcePropAccessor1);
                Action binding2 = CreateBinding(target, targetPropAccessor2, source, sourcePropAccessor2);
                Action binding3 = CreateBinding(target, targetPropAccessor3, source, sourcePropAccessor3);
                Action binding4 = CreateBinding(target, targetPropAccessor4, source, sourcePropAccessor4);
                Action binding5 = CreateBinding(target, targetPropAccessor5, source, sourcePropAccessor5);
                Action binding6 = CreateBinding(target, targetPropAccessor6, source, sourcePropAccessor6);
                Action binding7 = CreateBinding(target, targetPropAccessor7, source, sourcePropAccessor7);
                Action binding8 = CreateBinding(target, targetPropAccessor8, source, sourcePropAccessor8);

                parent.detectActions.Add(binding1);
                parent.detectActions.Add(binding2);
                parent.detectActions.Add(binding3);
                parent.detectActions.Add(binding4);
                parent.detectActions.Add(binding5);
                parent.detectActions.Add(binding6);
                parent.detectActions.Add(binding7);
                parent.detectActions.Add(binding8);

                return parent;
            }
        }
        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Core;
using ScenarioModel;

namespace UI.Animations
{
    public class AnimationCompiler
    {
        // name -> 生成関数（依存を渡して new）
        private readonly Dictionary<string, Func<IUIAnimation>> factories;

        public AnimationCompiler(IDisplayImage displayImage, IImageAdder imageAdder)
        {
            factories = new Dictionary<string, Func<IUIAnimation>>(StringComparer.OrdinalIgnoreCase)
            {
                ["slide"] = () => new Slide(displayImage),
                ["shake"] = () => new Shake(displayImage),
                ["image"] = () => new ImageAddAnimation(displayImage,imageAdder),
                ["chain"] = () => new AnimationChain(),
            };
        }

        public IEnumerable<IUIAnimation> CompileMany(IEnumerable<AnimationSpec> specs)
        {
            return specs.Select(Compile);
        }

        public IUIAnimation Compile(AnimationSpec spec)
        {
            if (!factories.TryGetValue(spec.Name, out var factory))
            {
                throw new InvalidOperationException($"未対応アニメ: {spec.Name}");
            }

            var instance = factory();

            if (instance is AnimationChain chain)
            {
                foreach (var child in spec.Children)
                {
                    var childAnim = Compile(child);
                    // 仕様上 chain の中に chain は来ない前提（来ても Children に入れるだけでOK）
                    chain.Children.Add(childAnim);
                }

                BindAttributes(chain, spec.Attrs);
                return chain;
            }

            BindAttributes(instance, spec.Attrs);
            return instance;
        }

        private static void BindAttributes(object target, IReadOnlyDictionary<string, string> attrs)
        {
            var props = target.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite)
                .ToDictionary(p => p.Name, p => p, StringComparer.OrdinalIgnoreCase);

            foreach (var (key, value) in attrs)
            {
                if (!props.TryGetValue(key, out var prop))
                {
                    continue;
                }

                prop.SetValue(target, ConvertString(value, prop.PropertyType));
            }
        }

        private static object ConvertString(string value, Type targetType)
        {
            var t = Nullable.GetUnderlyingType(targetType) ?? targetType;

            return t switch
            {
                _ when t == typeof(string) => value,
                _ when t.IsEnum => Enum.Parse(t, value, true),
                _ when t == typeof(bool) => bool.Parse(value),
                _ when t == typeof(int) => int.Parse(value, CultureInfo.InvariantCulture),
                _ when t == typeof(float) => float.Parse(value, CultureInfo.InvariantCulture),
                _ when t == typeof(double) => double.Parse(value, CultureInfo.InvariantCulture),
                _ => Convert.ChangeType(value, t, CultureInfo.InvariantCulture),
            };
        }
    }
}
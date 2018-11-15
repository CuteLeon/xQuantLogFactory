using System;
using System.Linq;
using System.Reflection;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace xQuantLogFactory.Utils.Extensions.Tests
{
    [TestClass()]
    public class DeepCloneExtensionTests
    {
        class BaseSeed
        {
            private static double Base_PrivateStaticDouble = 101.101;
            protected static double Base_ProtectedStaticDouble = 303.303;
            public static double Base_PublicStaticDouble = 202.202;

            private int Base_PrivateInt = 100;
            protected int Base_ProtectedInt = 300;
            public int Base_PublicInt = 200;

            private string Base_PrivateString = "hello";
            protected string Base_ProtectedString = "hello world";
            public string Base_PublicString = "WORLD";

            private string Base_PrivateStringProperty
            {
                get => this.Base_PrivateString;
                set => this.Base_PrivateString = value;
            }
            protected string Base_ProtectedStringProperty
            {
                get => this.Base_ProtectedString;
                set => this.Base_ProtectedString = value;
            }
            public string Base_PublicStringProperty
            {
                get => this.Base_PublicString;
                set => this.Base_PublicString = value;
            }

            private int Base_PrivateIntProperty
            {
                get => this.Base_PrivateInt;
                set => this.Base_PrivateInt = value;
            }
            protected int Base_ProtectedIntProperty
            {
                get => this.Base_ProtectedInt;
                set => this.Base_ProtectedInt = value;
            }
            public int Base_PublicIntProperty
            {
                get => this.Base_PublicInt;
                set => this.Base_PublicInt = value;
            }

            private static double Base_PrivateDoubleProperty
            {
                get => Base_PrivateStaticDouble;
                set => Base_PrivateStaticDouble = value;
            }
            protected static double Base_ProtectedDoubleProperty
            {
                get => Base_ProtectedStaticDouble;
                set => Base_ProtectedStaticDouble = value;
            }
            public static double Base_PublicDoubleProperty
            {
                get => Base_PublicStaticDouble;
                set => Base_PublicStaticDouble = value;
            }
        }

        class TestSeed : BaseSeed
        {
            private static double PrivateStaticDouble = 101.101;
            protected static double ProtectedStaticDouble = 303.303;
            public static double PublicStaticDouble = 202.202;

            private int PrivateInt = 100;
            protected int ProtectedInt = 300;
            public int PublicInt = 200;

            private string PrivateString = "hello";
            protected string ProtectedString = "hello world";
            public string PublicString = "WORLD";

            private string PrivateStringProperty
            {
                get => this.PrivateString;
                set => this.PrivateString = value;
            }
            protected string ProtectedStringProperty
            {
                get => this.ProtectedString;
                set => this.ProtectedString = value;
            }
            public string PublicStringProperty
            {
                get => this.PublicString;
                set => this.PublicString = value;
            }

            private int PrivateIntProperty
            {
                get => this.PrivateInt;
                set => this.PrivateInt = value;
            }
            protected int ProtectedIntProperty
            {
                get => this.ProtectedInt;
                set => this.ProtectedInt = value;
            }
            public int PublicIntProperty
            {
                get => this.PublicInt;
                set => this.PublicInt = value;
            }
            
            private static double PrivateDoubleProperty
            {
                get => PrivateStaticDouble;
                set => PrivateStaticDouble = value;
            }
            protected static double ProtectedDoubleProperty
            {
                get => ProtectedStaticDouble;
                set => ProtectedStaticDouble = value;
            }
            public static double PublicDoubleProperty
            {
                get => PublicStaticDouble;
                set => PublicStaticDouble = value;
            }
        }

        [TestMethod()]
        public void DeepCloneTest()
        {
            /* 成员 = 方法 并 属性 并 字段
             * 默认方法获取：
             *      => 子类公共成员+静态成员
             * BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public 获取：
             *      => 子类所有成员+基类非私有静态成员
             */
            Type seedType = typeof(TestSeed);

            Console.WriteLine("字段测试：");
            Console.WriteLine($"seedType.GetFields() \t: \t{string.Join("、", seedType.GetFields().Select(field => field.Name))}");
            Console.WriteLine($"seedType.GetFields(*) \t: \t{string.Join("、", seedType.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public).Select(field => field.Name))}");

            Console.WriteLine("属性测试：");
            Console.WriteLine($"seedType.GetProperties() \t: \t{string.Join("、", seedType.GetProperties().Select(property => property.Name))}");
            Console.WriteLine($"seedType.GetProperties(*) \t: \t{string.Join("、", seedType.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public).Select(property => property.Name))}");

            Console.WriteLine("成员测试：");
            Console.WriteLine($"seedType.GetMembers() \t: \t{string.Join("、", seedType.GetMembers().Select(member => member.Name))}");
            Console.WriteLine($"seedType.GetMembers(*) \t: \t{string.Join("、", seedType.GetMembers(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public).Select(member => member.Name))}");
        }
    }
}
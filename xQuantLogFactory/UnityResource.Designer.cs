﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace xQuantLogFactory {
    using System;
    
    
    /// <summary>
    ///   一个强类型的资源类，用于查找本地化的字符串等。
    /// </summary>
    // 此类是由 StronglyTypedResourceBuilder
    // 类通过类似于 ResGen 或 Visual Studio 的工具自动生成的。
    // 若要添加或移除成员，请编辑 .ResX 文件，然后重新运行 ResGen
    // (以 /str 作为命令选项)，或重新生成 VS 项目。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class UnityResource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal UnityResource() {
        }
        
        /// <summary>
        ///   返回此类使用的缓存的 ResourceManager 实例。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("xQuantLogFactory.UnityResource", typeof(UnityResource).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   重写当前线程的 CurrentUICulture 属性
        ///   重写当前线程的 CurrentUICulture 属性。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   查找类似 &lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot;?&gt;
        ///&lt;MonitorRoot xmlns:xsi=&quot;http://www.w3.org/2001/XMLSchema-instance&quot; xmlns:xsd=&quot;http://www.w3.org/2001/XMLSchema&quot; Name=&quot;监听客户端启动方案&quot;&gt;
        ///  &lt;Item Name=&quot;客户端启动&quot; Begin=&quot;登录成功&quot; End=&quot;初始化完成--&quot;&gt;
        ///    &lt;Item Name=&quot;数据加载&quot; Begin=&quot;加载中债参数设置表&quot; End=&quot;加载当前登录部门&quot;&gt;
        ///      &lt;Item Name=&quot;债券加载&quot; Begin=&quot;加载TBND查询&quot; End=&quot;加载TBND&quot; /&gt;
        ///    &lt;/Item&gt;
        ///    &lt;Item Name=&quot;额外空任务&quot; /&gt;
        ///  &lt;/Item&gt;
        ///  &lt;Item Name=&quot;直联服务&quot;&gt;
        ///    &lt;Item Name=&quot;直联服务1234&quot; Begin=&quot;直联服务1234|执行&quot; End=&quot;直联服务1234|结束&quot; /&gt;
        ///    &lt;Item Name=&quot;直联服务1194&quot; Begin=&quot;直联 [字符串的其余部分被截断]&quot;; 的本地化字符串。
        /// </summary>
        internal static string Monitor_Template {
            get {
                return ResourceManager.GetString("Monitor_Template", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找 System.Byte[] 类型的本地化资源。
        /// </summary>
        internal static byte[] xQuant_EXCEL_Templet {
            get {
                object obj = ResourceManager.GetObject("xQuant_EXCEL_Templet", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        /// <summary>
        ///   查找类似于 (图标) 的 System.Drawing.Icon 类型的本地化资源。
        /// </summary>
        internal static System.Drawing.Icon xQuantLogFactoryIcon {
            get {
                object obj = ResourceManager.GetObject("xQuantLogFactoryIcon", resourceCulture);
                return ((System.Drawing.Icon)(obj));
            }
        }
    }
}

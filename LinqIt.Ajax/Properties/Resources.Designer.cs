﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.235
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LinqIt.Ajax.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("LinqIt.Ajax.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
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
        ///   Looks up a localized string similar to if(!this.JSON){this.JSON={}}(function(){function f(a){return a&lt;10?&quot;0&quot;+a:a}function quote(a){escapable.lastIndex=0;return escapable.test(a)?&apos;&quot;&apos;+a.replace(escapable,function(a){var b=meta[a];return typeof b===&quot;string&quot;?b:&quot;\\u&quot;+(&quot;0000&quot;+a.charCodeAt(0).toString(16)).slice(-4)})+&apos;&quot;&apos;:&apos;&quot;&apos;+a+&apos;&quot;&apos;}function str(a,b){var c,d,e,f,g=gap,h,i=b[a];if(i&amp;&amp;typeof i===&quot;object&quot;&amp;&amp;typeof i.toJSON===&quot;function&quot;){i=i.toJSON(a)}if(typeof rep===&quot;function&quot;){i=rep.call(b,a,i)}switch(typeof i){case&quot;string&quot;:return quote(i);case&quot;number&quot;:ret [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string JSON2_Min {
            get {
                return ResourceManager.GetString("JSON2_Min", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to function callMethodAsync(control, method, onSuccess) {
        ///	var data = {};
        ///	data.control = control;
        ///	data.method = method;
        ///	data.args = {};
        ///	for (var i = 3; i &lt; arguments.length; i++) {
        ///		data.args[&quot;arg&quot; + (i - 3)] = arguments[i];
        ///	}
        ///	$.ajax({
        ///		type: &quot;POST&quot;,
        ///		url: document.location.pathname.replace(/\.[^/]+$/,&apos;&apos;) + &quot;/AjaxProxy.json&quot; + document.location.search,
        ///		data: JSON.stringify(data),
        ///		contentType: &quot;LinqIt/callback; charset=utf-8&quot;,
        ///		dataType: &quot;json&quot;,
        ///		success: onSuccess,
        ///		error: OnMeth [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Script {
            get {
                return ResourceManager.GetString("Script", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to function callMethodAsync(a,b,c){var d={};d.control=a;d.method=b;d.args={};for(var e=3;e&lt;arguments.length;e++){d.args[&quot;arg&quot;+(e-3)]=arguments[e]}$.ajax({type:&quot;POST&quot;,url:document.location.pathname.replace(/\.[^/]+$/,&quot;&quot;)+&quot;/AjaxProxy.json&quot;+document.location.search,data:JSON.stringify(d),contentType:&quot;LinqIt/callback; charset=utf-8&quot;,dataType:&quot;json&quot;,success:c,error:OnMethodFailed})}function callMethod(control,method){var data={};data.control=control;data.method=method;data.args={};for(var i=2;i&lt;arguments.length;i++ [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Script_Min {
            get {
                return ResourceManager.GetString("Script_Min", resourceCulture);
            }
        }
    }
}
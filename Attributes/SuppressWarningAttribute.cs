using System;
using System.Linq;

namespace AdvancedSceneManager.CsProjAttributes
{

    /// <summary>Provides support for specifying <see langword="&lt;SuppressWarning&gt;"/> in the generated csproj for a project.</summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class SuppressWarningAttribute : CsProjPropertyAttribute
    {

        public string warningCode { get; }

        public SuppressWarningAttribute(string code) =>
            warningCode = code;

        protected override void GenerateCsProjFragment(CsProjAttribute[] attributes)
        {

            var list = attributes.OfType<SuppressWarningAttribute>().Select(attr => attr.warningCode).ToList();
            list.Insert(0, "$(NoWarn)");

            var str = string.Join(";", list);

            Insert("NoWarn", str);

        }

    }

}

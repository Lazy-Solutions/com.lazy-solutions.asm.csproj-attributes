using System;
using System.Linq;

namespace AdvancedSceneManager.CsProjAttributes
{
    /// <summary>Provides support for specifying <see langword="&lt;Include&gt;"/> in the generated csproj for a project.</summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class IncludeAttribute : CsProjItemAttribute
    {

        public string pattern { get; }

        public IncludeAttribute(string pattern) =>
            this.pattern = pattern;

        protected override void GenerateCsProjFragment(CsProjAttribute[] attributes)
        {
            var includes = attributes.OfType<IncludeAttribute>().Select(attr => attr.pattern).ToList();
            foreach (var pattern in includes)
                Insert("None", ("Include", pattern));
        }

    }

}

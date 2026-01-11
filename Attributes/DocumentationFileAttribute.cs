using System;

namespace AdvancedSceneManager.CsProjAttributes
{

    /// <summary>Provides support for specifying <see langword="&lt;DocumentationFile&gt;"/> in the generated csproj for a project.</summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public sealed class DocumentationFileAttribute : CsProjPropertyAttribute
    {

        public string path { get; }

        public DocumentationFileAttribute(string path) =>
            this.path = path;

        protected override void GenerateCsProjFragment(CsProjAttribute[] attributes)
        {
            Insert("DocumentationFile", path);
        }

    }

}

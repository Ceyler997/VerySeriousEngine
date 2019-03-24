using SharpDX;
using System.Collections.Generic;
using VerySeriousEngine.Utils;

namespace VerySeriousEngine.Interfaces
{
    //
    // Summary:
    //     Interface, that should be implemented by all supposed to be rendered objects/components
    public interface IRenderable
    {
        bool IsRendered { get; }
        IEnumerable<RenderSetup> Setup { get; }
        Matrix WorldMatrix { get; }
    }
}

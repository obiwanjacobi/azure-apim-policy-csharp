namespace Jacobi.Azure.ApiManagement.Policy;

public abstract partial class PolicyFragment : PolicyDocumentBase
{
    protected void Comment(string comment)
    {
        Writer.Comment($" {comment} ");
    }

    protected abstract void Fragment(IPolicyFragment fragment);

    internal void WriteTo(Stream stream)
    {
        using var scope = CreateWriter(stream, isFragment: true);

        Writer.Fragment(() => Fragment(this));
    }
}

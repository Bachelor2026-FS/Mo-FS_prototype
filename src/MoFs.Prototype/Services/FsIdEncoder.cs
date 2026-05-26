using System.Text;

namespace MoFs.Prototype.Services;

public class FsIdEncoder
{
    public string Encode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Kan ikke encode en tom verdi.", nameof(value));
        }

        return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
    }
}

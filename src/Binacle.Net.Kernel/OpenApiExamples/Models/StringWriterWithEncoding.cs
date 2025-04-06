using System.Text;

namespace OpenApiExamples.Models;

internal class StringWriterWithEncoding : StringWriter
{
	public StringWriterWithEncoding(Encoding encoding)
	{
		this.Encoding = encoding;
	}

	public override Encoding Encoding { get; }
	
}

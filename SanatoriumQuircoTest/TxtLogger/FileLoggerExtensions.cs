using Microsoft.Extensions.Logging;

namespace SanatoriumQuircoTest.TxtLogger
{
	public static class FileLoggerExtensions
	{
		public static ILoggingBuilder AddFile(this ILoggingBuilder builder,
										string filePath)
		{
			builder.AddProvider(new FileLoggerProvider(filePath));
			return builder;
		}
	}
}

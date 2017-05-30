using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.Threading.Tasks;
using System.Text;
using Microsoft.Net.Http.Headers;
using VDS.RDF;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using VDS.RDF.Writing;

namespace ZWaveAdmin.Formatters
{
    public class RdfTurtleOutputFormatter : TextOutputFormatter
    {
        public RdfTurtleOutputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/turtle"));

            SupportedEncodings.Add(Encoding.UTF8);
        }

        protected override bool CanWriteType(Type type)
        {
            return typeof(IGraph).GetTypeInfo().IsAssignableFrom(type);
        }

        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            IServiceProvider serviceProvider = context.HttpContext.RequestServices;

            var response = context.HttpContext.Response;

            var graph = context.Object as IGraph;

            var writer = new CompressingTurtleWriter();
            var ttl = StringWriter.Write(graph, writer);

            return response.WriteAsync(ttl);
        }
    }
}

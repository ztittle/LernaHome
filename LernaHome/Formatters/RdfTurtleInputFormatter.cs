using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using VDS.RDF;
using System.Reflection;
using System.IO;

namespace ZWaveAdmin.Formatters
{
    public class RdfTurtleInputFormatter : TextInputFormatter
    {
        public RdfTurtleInputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/turtle"));

            SupportedEncodings.Add(Encoding.UTF8);
        }

        protected override bool CanReadType(Type type)
        {
            return typeof(IGraph).GetTypeInfo().IsAssignableFrom(type);
        }

        public override Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
        {
            var request = context.HttpContext.Request;
            string ttl;
            using (var body = request.Body)
            using (var streamReader = new StreamReader(body))
            {
                ttl = streamReader.ReadToEnd();
            }

            var graph = new Graph();
            graph.LoadFromString(ttl);

            return InputFormatterResult.SuccessAsync(graph);
        }
    }
}

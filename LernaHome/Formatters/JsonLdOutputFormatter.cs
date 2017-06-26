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
    public class JsonLdOutputFormatter : OutputFormatter
    {
        public JsonLdOutputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/ld+json"));
        }

        protected override bool CanWriteType(Type type)
        {
            return typeof(IGraph).GetTypeInfo().IsAssignableFrom(type);
        }

        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
        {
            IServiceProvider serviceProvider = context.HttpContext.RequestServices;

            var response = context.HttpContext.Response;

            var graph = context.Object as IGraph;

            var ldApi = new JsonLD.Core.JsonLdApi();
            var rdfDataset = new JsonLD.Core.RDFDataset();

            foreach (var triple in graph.Triples)
            {
                var ntf = new VDS.RDF.Writing.Formatting.NTriplesFormatter();
                if (triple.Object.NodeType == NodeType.Literal)
                {
                    var objNode = (ILiteralNode)triple.Object;
                    rdfDataset.AddTriple(triple.Subject.ToString(ntf).Trim('<','>'), triple.Predicate.ToString(ntf).Trim('<', '>'), objNode.Value, objNode.DataType?.OriginalString, string.IsNullOrEmpty(objNode.Language) ? null : objNode.Language);
                }
                else
                {
                    rdfDataset.AddTriple(triple.Subject.ToString(ntf).Trim('<', '>'), triple.Predicate.ToString(ntf).Trim('<', '>'), triple.Object.ToString(ntf).Trim('<', '>'));
                }
            }

            var ld = ldApi.FromRDF(rdfDataset);

            return response.WriteAsync(ld.ToString());
        }
    }
}

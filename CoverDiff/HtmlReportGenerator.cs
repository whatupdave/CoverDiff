using System;
using System.Collections.Generic;
using System.IO;
using System.Resources;
using System.Text;
using NHaml;

namespace CoverDiff
{
    internal class HtmlReportGenerator
    {
        // TODO: NHamlify when nhaml has support for embedded resources

        private readonly Stream _output;

        public HtmlReportGenerator(Stream outputStream)
        {
            _output = outputStream;
        }

        public void Report(CoverageComparer comparer)
        {
            var indexTemplate = ReadTemplate("index");
            var classTemplate = ReadTemplate("class"); ;
            var methodTemplate = ReadTemplate("method"); ;

            var templateValues = new Dictionary<string, string>();

            var classHtmlParts = new List<string>();
            foreach (var comparedClass in comparer.ComparedClasses)
            {
                var methodHtmlParts = GetMethodHtmlParts(comparedClass, methodTemplate).ToArray();
                classHtmlParts.Add(classTemplate
                   .Replace("#CLASSNAME#", comparedClass.Name)
                   .Replace("#CLASSID#", comparedClass.Name.Replace('.','_').ToLower())
                   .Replace("$METHODS$", string.Join("\n", methodHtmlParts)));
            }

            var message = string.Format("You have {0} {1} points ({2} local vs {3} prev)",
                                          comparer.CoveredPoints > 0 ? "covered" : "uncovered", Math.Abs(comparer.CoveredPoints),
                                          comparer.CurrentPoints, comparer.PrevPoints);

            var indexHtml = indexTemplate.Replace("#MESSAGE#", message)
                .Replace("$CLASSES$", string.Join("\n", classHtmlParts.ToArray()));

            using (var writer = new StreamWriter(_output))
            {
                writer.Write(indexHtml);
                writer.Flush();
            }
        }

        private List<string> GetMethodHtmlParts(ComparedClass comparedClass, string methodTemplate)
        {
            var methodHtmlParts = new List<string>();
            foreach (var comparedMethod in comparedClass.ComparedMethods)
            {
                var lineNumbersHtml = new StringBuilder();
                var codeLinesHtml = new StringBuilder();
                foreach (var comparedLine in comparedMethod.ComparedLines)
                {
                    var spanModifer = "";
                    if (comparedLine.State == LineState.Covered)
                        spanModifer = " class=\"covered\"";
                    else if (comparedLine.State == LineState.Uncovered)
                        spanModifer = " class=\"uncovered\"";

                    codeLinesHtml.AppendLine(string.Format
                                                 (@"<span{0}>{1}</span>", spanModifer, comparedLine.
                                                 LineNumber));
                    lineNumbersHtml.AppendLine(string.Format(
                                                   @"<span{0}>{1}</span>", spanModifer, comparedLine.CurrentLine.
                                                   Code));
                }
                methodHtmlParts.Add(methodTemplate
                    .Replace("$LINE_NUMBERS$", lineNumbersHtml.ToString())
                    .Replace("$CODE_LINES$", codeLinesHtml.ToString()));
            }
            return methodHtmlParts;
        }

        private string ReadTemplate(string template) 
        {
            using (var reader = new StreamReader(GetType().Assembly.GetManifestResourceStream(@"CoverDiff.Templates." + template + @".html")))
                return reader.ReadToEnd();
        }
    }
}
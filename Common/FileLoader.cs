using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


using ProgramTree;

namespace SimpleLang
{
    public static class FileLoader
    {
        public enum GrammarType { PASCAL, C, UNKNOWN }

        private static GrammarType GetGrammarType(string fileName)
        {
            string extension = fileName.Substring(fileName.LastIndexOf(".") + 1);
            if (extension == "pasn") return GrammarType.PASCAL;
            else if (extension == "cn") return GrammarType.C;
            return GrammarType.UNKNOWN;
        }

        public static BlockNode Parse(string fileName, Encoding encoding)
        {
            var type = GetGrammarType(fileName);
            if (type == GrammarType.UNKNOWN)
                throw new Exception("File extension is unknown");
            if (type == GrammarType.C)
            {
                string content = File.ReadAllText(fileName);                
                SimpleScannerC.Scanner scanner = new SimpleScannerC.Scanner();
                scanner.SetSource(content, 0);
                SimpleParserC.Parser parser = new SimpleParserC.Parser(scanner);

                //Everything's okay here: exception handling in Main will handle all errors,
                //so we don't need to check the value;
                if (parser.Parse())
                    return parser.root;
                else
                    throw new Exception("Parsing error. ");
            } else {
                string content = File.ReadAllText(fileName);
                SimpleScannerPascal.Scanner scanner = new SimpleScannerPascal.Scanner();
                scanner.SetSource(content, 0);
                SimpleParserPascal.Parser parser = new SimpleParserPascal.Parser(scanner);

                //Everything's okay here: exception handling in Main will handle all errors,
                //so we don't need to check the value;
                if (parser.Parse())
                    return parser.root;
                else
                    throw new Exception("Parsing error. ");
            }
        }
    }
}

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

        public static GrammarType GetGrammarType(string fileName)
        {
            string extension = System.IO.Path.GetExtension(fileName);
            if (extension == ".pasn") return GrammarType.PASCAL;
            else if (extension == ".cn") return GrammarType.C;
            return GrammarType.UNKNOWN;
        }

        public static BlockNode LoadFile(string fileName, Encoding encoding)
        {
            string content = File.ReadAllText(fileName, encoding);
            var type = GetGrammarType(fileName);
            return Parse(content, type);
        }

        public static BlockNode Parse(string content, GrammarType type)
        {
            switch (type)
            {
                case GrammarType.UNKNOWN:
                    throw new Exception("File extension is unknown");
                case GrammarType.C:
                    {
                        SimpleScannerC.Scanner scanner = new SimpleScannerC.Scanner();
                        scanner.SetSource(content, 0);
                        SimpleParserC.Parser parser = new SimpleParserC.Parser(scanner);

                        //Everything's okay here: exception handling in Main will handle all errors,
                        //so we don't need to check the value;
                        if (parser.Parse())
                            return parser.root;
                        else
                            throw new Exception("Parsing error. ");
                    }
                case GrammarType.PASCAL:
                    {
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
                default:
                    //Simply cannot happen.
                    return null;
            }                        
        }            
    }
}

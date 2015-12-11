DocumentationAssembler
cd Pictures
"C:\Program Files (x86)\Pandoc\pandoc" --latex-engine=xelatex -V mainfont="Sitka Text" --highlight-style tango ..\OptimizedCompilersProjectDocumentation.txt -o ..\OptimizedCompilersProjectDocumentation.pdf
cd ..
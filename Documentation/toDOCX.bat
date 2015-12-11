DocumentationAssembler
cd %~dp0
cd Pictures
"C:\Program Files (x86)\Pandoc\pandoc" --latex-engine=xelatex -V mainfont="Sitka Text" --highlight-style tango ..\OptimizedCompilersProjectDocumentation.txt -o ..\OptimizedCompilersProjectDocumentation.docx
cd ..
del OptimizedCompilersProjectDocumentation.txt

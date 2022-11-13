FUNC main
VARIABLE GOFUNC a
VARIABLE txt1 Path: 
VARIABLE txt2 Text: 
PRINT txt1
INPUTSTR path
PRINT txt2
INPUTSTR text
VARIABLE int 0
IO FILE LENGTH path len
IFSTR GOFUNC = GOFUNC start
FUNC start
IFINT int = len end
IO FILE READ path int ln
::VARIABLE txt13 #int^^^|^^^#len^^^|^^^#path^^^|^^^#ln
::PRINT txt13
::PRINT;
IFSTR ln = text found
VARIABLE val1 1
MATH int + val1 int
IFSTR GOFUNC = GOFUNC start
FUNC end
VARIABLE msg Not found text :(
PRINT msg
PRINT;
INPUT;
EXIT
FUNC found
VARIABLE txt34 Success! Index = ^^^#int^^^; Text = ^^^#text
PRINT txt34
PRINT;
INPUT;
EXIT
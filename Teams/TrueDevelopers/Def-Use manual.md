## Пояснение по работе Def-Use анализа переменных

Анализ представлен тремя методами класса Block:

1. `void CalculateDefUseData()`, проходящую по всем строкам 3-х адресного кода данного блока и заполняющую внутреннюю структуру живыми переменными для каждой строки 3-х адресного кода. Метод должен вызываться перед первым вызовом `IsVariableAlive`, `GetAliveVariables`, также требуется повторный вызов, если блок был изменен.

2. `bool IsVariableAlive(string variable, int step)`, возвращает false, если переменная мертва на данном шаге, в противном случае 1.

3. `HashSet<string> GetAliveVariables(int step)`, возвращает множество живых переменных для заданного шага.

```cs
codeGenerator.Code.blocks[0].CalculateDefUseData();
for (int i = 0; i < codeGenerator.Code.blocks[0].Count; i++)
{
	foreach (string variable in codeGenerator.Code.blocks[0].GetAliveVariables(i))
	{
		Console.Write(variable + " ");
	}
        
	Console.WriteLine();
}
```

Пример использования есть в `Main.cs`, см.  `//DEBUG def-use data view` (где-то в районе 51-й строки)

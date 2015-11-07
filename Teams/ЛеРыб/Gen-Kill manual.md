## Пояснение по работе генерации Gen-Kill множеств для достигающих определений
Генерация представлена вызовом метода класса ThreeAddrCode `GetGenKillInfoData()`, возвращающего `List<GenKillInfo>`.
Каждый элемент списка представляет собой множества Gen и Kill для соответствующего блока.

Класс `GenKillInfo` содержит два поля `HashSet<Index>` с именами `Gen` и `Kill`, представляющие соответствующие множества для блока.

Класс Index содержит информацию о переменной из множеств `Gen-Kill`, содержит поля `int mBlockInd` с номером блока,
`int mInternalInd` - номер строки в блоке, `mVariableName` - имя переменной.

Пример использования:
```cs
   Console.WriteLine("GenKillInfo");
   var a = codeGenerator.Code.GetGenKillInfoData();
   for (int i = 0; i < a.Count; ++i)
   {
		Console.WriteLine("Block: " + i);
		Console.WriteLine("Gen");
		foreach (ThreeAddrCode.Index ind in a[i].Gen)
			Console.WriteLine(ind.ToString());
			Console.WriteLine("Kill");
			foreach (ThreeAddrCode.Index ind in a[i].Kill) {
				Console.WriteLine(ind.ToString());
			}
			Console.WriteLine();
   }
```

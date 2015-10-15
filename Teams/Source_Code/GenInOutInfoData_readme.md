Итерационный алгоритм для достигающих определений:

Использование:

	Console.WriteLine("InOutInfo");
	var a = codeGenerator.Code.GetInOutInfoData();
	for (int i = 0; i < a.Count; ++i)
	{
		Console.WriteLine("Block: " + i);
		Console.WriteLine("In");
		foreach (ThreeAddrCode.Index ind in a[i].In)
			Console.WriteLine(ind.ToString());
		Console.WriteLine("Out");
		foreach (ThreeAddrCode.Index ind in a[i].Out)
			Console.WriteLine(ind.ToString());
		Console.WriteLine();
	}
	
Реализация:
	Методы ThreeAddrCode::GetInOutInfoData() возвращает List<InOutInfo>, где индекс соответствует номеру блока
	Класс InOutInfo содержит поля:
		In - множество In блока
		Out - множество Out блока
	, которые содержат элементы Index, соответствующие определниям (см. документацию по GetGenKillInfoData())
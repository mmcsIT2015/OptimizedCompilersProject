������������ �������� ��� ����������� �����������:

�������������:

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
	
����������:
	������ ThreeAddrCode::GetInOutInfoData() ���������� List<InOutInfo>, ��� ������ ������������� ������ �����
	����� InOutInfo �������� ����:
		In - ��������� In �����
		Out - ��������� Out �����
	, ������� �������� �������� Index, ��������������� ����������� (��. ������������ �� GetGenKillInfoData())
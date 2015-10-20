### Оптимизация: Устранение общих подвыражений


> Примечание: Использовать после разбиения кода на внутренние блоки

Пример использования:
```cs
	CommonSubexpressionsOptimization opt = new CommonSubexpressionsOptimization(codeGenerator.Code);
	opt.Optimize();
	Console.WriteLine(codeGenerator.Code);
```

Пример оптимизации:
```
a = b + c
b = a - d
c = b + c
k = 4 + 10
d = a - d
l = a - d
m = a - d
```
_________________________________
```
a = b + c
b = a - d
c = b + c
k = 4 + 10
d = b
l = a - d
m = l
```

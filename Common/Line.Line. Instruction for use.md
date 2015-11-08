# Инструкция по использованию `Line.Line`

Ниже перечислены основные, но не единственные методы Line.Line (далее - просто Line, пространство имен будет опускаться).
```cs
public class Line
{
  public string label = "";

  public virtual bool IsEmpty(); // является ли строка пустым оператором
  public virtual bool HasLabel(); // есть ли у строки метка

  /// Явялется ли класс строкой перехода (GoTo), вызовом функции (FunctionCall),
  /// операцией вида `x + y + z` (Operation) и т.п.
  /// Пример:
  /// if (line.Is<Line.Operation>())
  ///   Console.Write("`line` is Line.Operation!");
  public bool Is<T>() where T : Line;

  /// Аналогично предыдущей, но возвращает обратный результат
  public bool IsNot<T>() where T : Line;
}
```

```cs
/// Класс для пустого оператора. Если в коде нужно создать подобный,
/// создаем его с помощью `new Line.EmptyLine()``
class EmptyLine : Line
{

}
```

```cs
/// Строка с командой `goto <L>` в трехадресном коде
class GoTo : NonEmptyLine
{
  public string target; // метка, на которую нужно перейти
}
```

```cs
/// Условный переход вида `if <cond> goto <L>` в трехадресном коде
class СonditionalJump : GoTo
{
  public string condition; // имя переменной, содержащей условие перехода
}
```

```cs
/// Параметр для последующего вызова функции, пример:
/// param <first-param>
/// param <second-param>
/// < вызов функции >
class FunctionParam : NonEmptyLine
{
  public string param; // имя переменной, содержащей значение параметра
}
```

```cs
/// Команда вызова функции в трехадресном коде
/// call <func-name>, <num-of-params>
class FunctionCall : NonEmptyLine
{
  public string name; // имя функции
  public int parameters; // число параметров (команд `param <param>` перед вызовом ф-и)
  public string destination; // возвращаемый параметр (если он есть)

  public virtual bool IsVoid(); // есть ли у функции возвращаемый параметр
}
```

```cs
/// Основная команда, которая понадобится для оптимизаций
/// `left` = `first` [`operation` `second`]
class Operation : NonEmptyLine
{
  public string left;
  public string first;
  public string second;
  public BinaryOperation operation;

  public virtual bool IsIdentity(); // является ли строка тождеством (a = b)

  /// является ли строка арифметическим выражением (операции +,-,*,/)
  public virtual bool IsArithmExpr();

  /// является ли строка логическим выражением (операции <= >=, <, >, !=)
  public virtual bool IsBoolExpr()

  public virtual bool FirstParamIsNumber(); // является ли первый параметр правой части числом
  public virtual bool SecondParamIsNumber(); // является ли второй параметр правой части числом

  /// Преобразует правую часть выражения в указанное значение
  /// (т.е. преобразует линию в тождество).
  /// Пример:
  /// line = `a = b + c`
  /// line.ToIdentity("z") -> `a = z`
  public void ToIdentity(string value);
}
```

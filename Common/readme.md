## Общие вопросы

В этой папке будут лежать файлы, общие для всех команд - возможно, это будет и основной проект, который будем собирать из частей от разных команд

Убедительно предлагается использовать в качестве основы именно предложенные здесь файлы (на данный момент это ProgramTree.cs) вместо собственных вариаций - во избежание.
В противном случае в будущем при слиянии однозначно возникнут серьезные проблемы.

Теперь по поводу предлагаемого варианта.

- Вопрос с именованием _переменных-членов класса_ остается открытым. Хорошей практикой считается все-таки различия в именовании таких переменных и именовании методов класса. Высказываем свои соображения в коментариях ниже (во избежание последующего недовольства при принудительно навязанном стиле именования).

- Пока вариантом с узлом для цикла оставлен (как первый предложенный вариант) CycleNode, в котором содержится переменная, определяющая тип цикла (while, do-while и т.п.). Но т.к. мы все таки пишем на C#, в котором ООП только приветствуется, лично я больше склоняюсь к варианту использования различных классов для различных типов цикла (возможно, наследованных от общего CycleNode) - WhileNode и DoWhileNode. В будущем, в зависимости от того, как эти классы будут использоваться, уже определимся более точно.

> Напоследок: не пренебрегайте пробелами/отступами, не лепите кучу инструкций на одной строке - это нечитаемо. Опять же мы пишем на C#, в котором лишние отступы принято активно использовать (даже слишком часто, чем это порой стоило бы делать).

## Пояснения по ThreeAddrCode

Класс `ThreeAddrCode` представляет собой всю программу в трехадресном коде. Предполагается, что именно его будет возвращать `Gen3AddrCodeVisitor` после обхода дерева. Основной массив - массив блоков. В каждом блоке содержатся, очевидно, строки трехадресных команд. Если разбиения на блоки нет - предполагается, что существует один глобальный блок.

- Первый блок создается автоматически;
- Пустым блок быть не может.

В будущем словарь `labels` будет содержать адреса меток по их идентификаторам.

Конструкции
```
L: x = y `op` z
```
соответствует
```
`label`: `left` = `first` `command` `second`
```
где в апострофах перечислены названия соответствующих переменных из структуры Line.

Пример трехадресного кода и создания для него `ThreeAddrCode`:
```ini
C: x = y + z
x = -z

x = y * 3
x = -3
```

```cs
ThreeAddrCode code = new ThreeAddrCode();
code.AddLine(new ThreeAddrCode.Line("x", "y", "+", "z")).label = "C";
code.AddLine(new ThreeAddrCode.Line("x", "-", "z"));
code.NewBlock();
code.AddLine(new ThreeAddrCode.Line("x", "y", "*", "2"));
code.AddLine(new ThreeAddrCode.Line("x", "-", "3"));

Console.WriteLine(code);
```

## Принципы работы

Итак, теперь файлы для общего проекта добавлены. Если кто не понял, лежит общий проект в папке Common. Общими считаются лишь файлы, непосредственно относящиеся к проекту на `C#`. Файлы `.lex` и `.y` у каждого человека/команды свои, и расположены в соответствующих папках. При этом после генерации на их основе некоторых исходных файлов для проекта этот самый проект должен собираться - так что если в грамматике есть недочеты/несоответствия, устраняем их, исходя из представленной версии `ProgramTree.cs` (в частности,это касается названий классов).

> На всякий случай, пример грамматики (`C`) расположен также в папке `Common` - повторюс, лишь пример.

Генерация исходных файлов из `.lex`/`.y` осуществляется каждым человеком/командой из своей папки - скрипт генерации переписан. Он будет генерировать файлы в общую папку проекта - т.е. сделали pull, сгенерировали для себя файлы (из своей папки, путем запуска скрипта) и спокойно работаем с общим проектом.

> Скрипт основан на текущей вложенности папок - т.е. если расположение файла generateParserScanner будет изменено, нужно будет поправить внутри него переменную `target`. Так, версия `set target=..\..\..\Common` корректна для расположения в папке `OptimizedCompilersProject\Teams\TrueDevelopers\task-1` - от этого и отталкиваемся.

## Реализация различных оптимизаций

Для каждой оптимизации предлагается использовать отдельный класс, наследующий `IOptimizer`. В качестве конструктора (желательно) наследник должен принимать экземпляр `ThreeAddrCode`, метод `Optimize()`, как следует из названия, выполняет оптимизацию.

Для первого задания располагаем вновь созданные классы-оптимизаторы в папке `Optimization/task-1`. По-возможности, даем этим классам осмысленные имена.

## Тесты

Реализована следующая возможность. При запуске скрипта сборки `.lex`/`.y` файлов в код на `C#` в запускаемой директории, и всех ее поддиректорих производится поиск файлов вида `test-*`, и список этих файлов копируется во вспомогательный файл в основном проекте. Затем, при запуске основного проекта (из `Common`) будет произведена обработка всех файлов из получившегося списка. Если подобных файлов найдено не было, все будет обрабатываться как и раньше. Это удобно для тестирования сразу нескольких вариантов.

Т.е. достаточно создать фай вида `test-*` в своей директории, и он будет проверяться основным проектом при запуске.Само собой, этот список нужно сформировать с помощью скрипта (после генерации он создается автоматически, отдельно его (список тестов) можно создать скриптом `collectTests`).

## Комментарии по расположению в трехадресном коде различных конструкций

Ниже приводится описание расположения элементов различных конструкций (goto, if и т.п.) в переменных внутри `ThreeAddrCode::Line`.
```cs
public class Line
{
    public string label;
    public string left;
    public string command;
    public string first;
    public string second;
    ...
}
```

>Далее в апострофах будут заключены все участвующие в трехадресном коде элементы

```
`if`        `expr` goto   `<label>`
 ^ command    ^ left         ^ first
```
```
`goto`    `<label>`
 ^ command    ^ left
```
```
/* Параметр вызываемой функции */
`param`     `<param-name>`
 ^ command    ^ left
```
```
/* Вызов функции-statement'а (`left` = "") */
`call`      `<func-name>` `<num-params>`
 ^ command    ^ first       ^ second
```
```
/* Вызов функции-expression'а */
`<dest>` = `call`		`<func-name>`,   `<num-params>`
 ^left	    ^ command     ^ first          ^ second
```


## OptimizedCompilersProject
Optimized compilers project

## Команды

1. __DoubleK__ - Папиж, Гаджиев
2. __TrueDevelopers__ - Пыслару, Михайличенко, Леонтьев
3. __Source_Code__ - Пак, Гончаров
4. __Стрела в колено__ - Троицкий, Проскуряков
5. __Ultimate__ - Батраков, Тухиков
6. __Альфа__ - Валяев, Гулканян


## Рекомендации
Базовые правила пользования гитхабом:

1. Для удобного использования предлагается скачать __GitExtensions__, __SourceTree__ или __GitHub for Windows__. Впрочем, консоль тоже не так плоха - особенно в Windows 10.

  > А вообще, курим маны по консоли.
  
  > Книга по работе с Git (GitHub) http://git-scm.com/book/ru/v1. Для полноценной работы и понимания принципа системы Git достаточно будет прочитать первые 3 главы.

2. Один коммит - одно изменение. Нужно затем, чтобы откат коммита не создавал проблем с переписыванием кучи различных изменений. Плюс, проще искать баги. Однако, это не значит, что после изменения каждой строчки нужно делать коммит - под изменением понимаем некоторую задачу.

3. Т.к. у нас общий репозитарий, сливать придется часто. Предлагается хранить ЛОКАЛЬНЫЕ коммиты, заливаем на паре/раз в неделю - иначе слиянием будем по часу заниматься, в случае большого количества коммитов.

4. Опять же исходя из общности репозитарий, рекомендуется отдавать приоритет команде `git pull --rebase` обычной команде `git pull` - это позволит избежать частых слияний, когда можно обойтись прстым перестроением дерева истории изменений (причем - автоматическим).

5. Имена и описания коммитов на английском языке.

6. Длина заголовка коммита - не более 80 символов. Более подробно описываем после заголовка, перейдя на новую строку. Почему - узнаете, если не будете придерживаться этого правила...

Остальное в процессе.

> Было бы неплохо, если все текстовые файлы имели кодировку UTF-8 - на случай, если придется пользоваться в качестве редактора не только Студией. При этом Студия создает файлы в ANSI - помните об этом.

Также для единообразия кода в проекте:

1. Т.к. до-диез, предлагается
  - использовать CamelCase для названия функций/методов/свойств;
  - использовать mCamelCase (т.е. префикс m) для переменных-членов класса;
  - для локальных переменных использовать camelCase;
  - избегать создания публичных членов класса - для этого есть свойства;
  - что касается остального - смотрим C# Coding Conventions.
2. Меньше обычных комментариев, больше документирующих(///).

Ссылки:
1. https://msdn.microsoft.com/en-us/library/ff926074.aspx - C# Coding Conventions
2. Используем константы вместо магических чисел, подпрограммы - как можно меньше по размеру, каждая публичная - с документирующим комментарием (с описанием параметров и результата).
3. Думаем о модифицируемости и масштабируемости кода, не пишем ересь, которая работает только для массивов размера 5. У нас IEnumerable<> и List<> есть.


## Задания (в пределах базового блока) (25.10.15)

1. Def-Use - информация о переменных ( __ЛеРыб__)
2. Устранение мёртвого кода (__DoubleK__)
3. Свёртка констант и алгебраические тождества (__Стрела в колено__)
4. Оптимизация общих подвыражений (__Source_Code__)
5. Протяжка костант (__Ultimate__)
6. Генерация 3-х адресного кода (__TrueDevelopers__)
7. Выделение базовых блоков (__Альфа__)

## Достигающие определения

1. Вычисление множеств genB и killB для достигающих определений ( __ЛеРыб__) (2.10.15)
2. Реализация передаточной функции ББ для достигающих определений: 
  - В виде суперпозиции передаточных функций команд (__DoubleK__)  (2.10.15)
  - По общей формуле (__TrueDevelopers__)  (2.10.15)
3. Итерационный алгоритм для достигающих определений (__Source_Code__) (9.10.15)
4. Подбор системы тестов для достигающих определений (__Ultimate__) (9.10.15)

## Активные переменные и доступные выражения (16.10.15)

1. Анализ активных переменных (__Стрела в колено__)
2. Набор тестов для задания 1 (__Стрела в колено__)
3. Анализ доступных выражений (__TrueDevelopers__)
4. Набор тестов для задания 3 (__Альфа__)
5. Оптимизации, связанные с заданием 1 (__DoubleK__)
6. Оптимизации, связанные с заданием 3 (__Ultimate__)

## (23.10.15)

1. Объединение трёх задач (__TrueDevelopers__)
  - активные переменные
  - достигающие определения
  - доступные выражения
  - распространение констант (__Ultimate__) (6.11.15)
2. GUI (__Альфа__)
3. Генерация IL-кода (__Source_Code__)

## Рефакторинг кода

1. Общий рефакторинг кода (__TrueDevelopers__) (30.10.15)
  - Преобразование графа потока управления (__Альфа__) (6.11.15)

## 20.11.15

1. Итерационный алгоритм для отношения dom (__TrueDevelopers__)
2. Построение графа dom (__Ultimate__)
3. Обход в глубину с нумерацией (__Альфа__)
4. Построение остовного дерева (__Альфа__)
5. Классификация рёбер (__Стрела в колено__)
6. Определить обратныерёбра в CFG (__DoubleK__)
7. Определить, является ли граф приводимым (__DoubleK__)
8. Ввести goto в грамматики (C, Pascal) (__Source_Code__)

## 27.11.15

1. Определение всех естественных циклов (__Альфа__)
2. Определение вложенности естественных циклов (__DoubleK__)
3. Определение глубины CFG (__Стрела в колено__)
4. Изменение итерационного алгоритма (__TrueDevelopers__)
5. Документация (__Альфа__)

## 4.12.15
1. Выделение областей (__Source_Code__)
2. Реализация алгоритма на основе областей

# 25.12.15. Сдача проекта

## Оценивание вклада
https://docs.google.com/spreadsheets/d/1kz_l02P7xZTzKq8kvne4xIBotsjbPTW8zV1JeOcspEI/edit?usp=sharing

## Деление бюджета
https://docs.google.com/spreadsheets/d/1Dh1GuR8s3RbV2dRjnpvORUo6VNIyjc93d2Q_cUQX-rA/edit?usp=sharing

## Заполните форму
https://docs.google.com/forms/d/1aaFplsmf_d55edBGKnXANV8FEzGhbHXul3PkiDcDOhc/viewform

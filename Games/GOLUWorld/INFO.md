## Блоки
* '.' - Пустота
* '#' - Блок металла (блок)
* 'P' - Доски (пол)
* 'A' - Асфальт (пол)
* 'B' - Кирпичи (блок)
* 'S' - Песок (пол)
* 'W' - Вода (блок)
* 'b' - Чёрный блок (блок)
* '^' - Трава (пол)
* 'C' - Бетонная балка (блок)
* 'Д' - [ГЕНЕРАТОР] Случайно, трава или пустота
* 'П' - [ГЕНЕРАТОР] Случайно, песок или пустота
* 'Ũ' - [ГЕНЕРАТОР] Генерирует стену или пол (уникально для структуры), доски или кирпичи
* 'ũ' - [ГЕНЕРАТОР] Генерирует стену или пол (уникально для структуры), доски или кирпичи (2-й вариант)

## Карта блоков

```
['.'] = 1,
['#'] = 2,
['A'] = 3,
['B'] = 4,
['S'] = 5,
['W'] = 6,
['Д'] = 7,
['^'] = 8,
['П'] = 9,
['C'] = 10,
['P'] = 11,
['Ũ'] = 12,
['ũ'] = 13,

new KeyValuePair<byte, ColorB>(1 , new ColorB(0, 0, 0, 0)),
new KeyValuePair<byte, ColorB>(2 , new ColorB(0, 0, 0, 255)),
new KeyValuePair<byte, ColorB>(3 , new ColorB(63, 63, 63, 255)),
new KeyValuePair<byte, ColorB>(4 , new ColorB(127, 127, 127, 255)),
new KeyValuePair<byte, ColorB>(5 , new ColorB(191, 191, 191, 255)),
new KeyValuePair<byte, ColorB>(6 , new ColorB(255, 0, 0, 255)),
new KeyValuePair<byte, ColorB>(7 , new ColorB(0, 255, 0, 128)),
new KeyValuePair<byte, ColorB>(8 , new ColorB(0, 255, 0, 255)),
new KeyValuePair<byte, ColorB>(9 , new ColorB(191, 191, 191, 128)),
new KeyValuePair<byte, ColorB>(10, new ColorB(198, 151, 151, 255)),
new KeyValuePair<byte, ColorB>(11, new ColorB(196, 149, 100, 255)),
new KeyValuePair<byte, ColorB>(12, new ColorB(161, 137, 113, 255)),
new KeyValuePair<byte, ColorB>(13, new ColorB(148, 133, 118, 255)),
```

## Сущности
* '.' - Пустота
* 'C' - Стул
* 'T' - Стол
* '^' - Шипы
* 's' - Паук (моб)
* '!' - Дерево
* '#' - Ящик (толкаемый блок)
* '~' - Трава
* '3' - Куст
* 'G' - Могила
* 'w' - Окно (блок)
* 'Д' - [ГЕНЕРАТОР] Случайно, дерево или трава или палка или куст или пустота
* 'д' - [ГЕНЕРАТОР] Случайно, трава или камень или куст или пустота
* 'т' - [ГЕНЕРАТОР] Случайно, трава или высокая трава или пустота
* 'М' - [ГЕНЕРАТОР] Случайная мебель, для дома или пустота
* 'м' - [ГЕНЕРАТОР] Различный мусор или пустота

## Карта сущностей

```
['.'] = 1,
['w'] = 2,
['М'] = 3,
['м'] = 4,
['т'] = 5,
['д'] = 6,
['Д'] = 7,

new KeyValuePair<byte, ColorB>(1 , new ColorB(0, 0, 0, 0)),
new KeyValuePair<byte, ColorB>(2 , new ColorB(109, 196, 255, 255)),
new KeyValuePair<byte, ColorB>(3 , new ColorB(242, 184, 125, 255)),
new KeyValuePair<byte, ColorB>(4 , new ColorB(239, 135, 119, 255)),
new KeyValuePair<byte, ColorB>(5 , new ColorB(0, 255, 0, 255)),
new KeyValuePair<byte, ColorB>(6 , new ColorB(89, 255, 0, 255)),
new KeyValuePair<byte, ColorB>(7 , new ColorB(182, 255, 0, 255)),
```

## Потолки
* '.' - Пустота
* '_' - Невидимый
* 'C' - Бетон
* 'R' - Черепица
* 'r' - Черепица (повёрнутая вверх)
* 'Ũ' - [ГЕНЕРАТОР] Генерирует черепицу или пустоту (уникально для структуры), доски или кирпичи
* 'ũ' - [ГЕНЕРАТОР] Генерирует черепицу (повёрнутая вверх) или пустоту (уникально для структуры), доски или кирпичи

## Карта потолков

```
['.'] = 1,
['_'] = 2,
['C'] = 3,
['R'] = 4,
['r'] = 5,
['Ũ'] = 6,
['ũ'] = 7,

new KeyValuePair<byte, ColorB>(1 , new ColorB(0, 0, 0, 0)),
new KeyValuePair<byte, ColorB>(2 , new ColorB(255, 255, 255, 255)),
new KeyValuePair<byte, ColorB>(3 , new ColorB(198, 151, 151, 255)),
new KeyValuePair<byte, ColorB>(4 , new ColorB(255, 86, 86, 255)),
new KeyValuePair<byte, ColorB>(5 , new ColorB(255, 170, 170, 255)),
new KeyValuePair<byte, ColorB>(6 , new ColorB(255, 86, 86, 200)),
new KeyValuePair<byte, ColorB>(7 , new ColorB(255, 170, 170, 200)),
```

## Коллизии
* L1 - Мир и игрок
* L2 - Наносит урон если ходить в нём
* L3 - Наносит всегда урон
* L4 - 
* L5 - Толкаемый блок
* L6 - Получаемое урон
BezierControl
=============

Class for drawing cubic bezier curve and mouse control using WinForms.

Класс для рисования кубической кривой Безье при помощи мыши.
Отрисовка кривой реализована встроенными средствами System.Drawing.Drawing2D, метод [GraphicsPath.AddBeziers](http://msdn.microsoft.com/en-us/library/wdba9had(v=vs.110).aspx).

###Управление
* Якоря (красные точки) и контролы (зеленые точки) можно таскать, зажав левую кнопку мыши.
* Нажать правую кнопку мыши в свободно месте холста, чтобы добавить новый якорь в конец кривой. 
* Нажать левую кнопку мыши на участке кривой для вставки нового якоря в позицию курсора мыши. 
* Нажать колесо мыши для удаления якоря в позиции курсора мыши. 



###Точки кривой
```
//Список точек кривой попиксельно
List<Point> pixels = bezier.ToPointList();
//Список точек полилинии, сформированной из кривой методом GraphicsPath.Flatten.
List<Point> polyline = bezier.ToPolyline();
```

![Demo Screenshot](https://dl.dropboxusercontent.com/u/1288526/bezir.png)
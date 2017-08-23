BezierControl
=============

Class for drawing cubic bezier curve with mouse, for use in WinForms.

Класс для рисования кубической кривой Безье при помощи мыши.
Отрисовка кривой реализована встроенными средствами System.Drawing.Drawing2D, метод [GraphicsPath.AddBeziers](http://msdn.microsoft.com/en-us/library/wdba9had(v=vs.110).aspx).

### Управление
* Якоря (красные точки), контролы (зеленые точки) и сегменты кривой можно таскать, зажав левую кнопку мыши.
* Нажать правую кнопку мыши в свободном месте холста или на сегменте кривой, чтобы добавить новый якорь в позицию курсора мыши. 
* Нажать колесо мыши для удаления якоря или сегмента кривой в позиции курсора мыши. 



### Точки кривой
```
//Список точек кривой попиксельно
List<Point> pixels = bezier.ToPixels;
//Список точек полилинии, сформированной из кривой методом GraphicsPath.Flatten.
List<Point> polyline = bezier.ToPolyline();
```

![Demo Screenshot](http://images.illuzor.com/uploads/bez-new.png)

BezierControl
=============

Class for drawing cubic bezier curve and mouse control using WinForms.

����� ��� ��������� ���������� ������ ����� ��� ������ ����.
��������� ������ ����������� ����������� ���������� System.Drawing.Drawing2D, ����� [GraphicsPath.AddBeziers](http://msdn.microsoft.com/en-us/library/wdba9had(v=vs.110).aspx).

###����������
* ����� (������� �����) � �������� (������� �����) ����� �������, ����� ����� ������ ����.
* ������ ������ ������ ���� � �������� ����� ������, ����� �������� ����� ����� � ����� ������. 
* ������ ����� ������ ���� �� ������� ������ ��� ������� ������ ����� � ������� ������� ����. 
* ������ ������ ���� ��� �������� ����� � ������� ������� ����. 



###����� ������
```
//������ ����� ������ �����������
List<Point> pixels = bezier.ToPointList();
//������ ����� ���������, �������������� �� ������ ������� GraphicsPath.Flatten.
List<Point> polyline = bezier.ToPolyline();
```

![Demo Screenshot](https://dl.dropboxusercontent.com/u/1288526/bezir.png)
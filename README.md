BezierControl
=============

Class for drawing cubic bezier curve with mouse, for use in WinForms.

����� ��� ��������� ���������� ������ ����� ��� ������ ����.
��������� ������ ����������� ����������� ���������� System.Drawing.Drawing2D, ����� [GraphicsPath.AddBeziers](http://msdn.microsoft.com/en-us/library/wdba9had(v=vs.110).aspx).

###����������
* ����� (������� �����), �������� (������� �����) � �������� ������ ����� �������, ����� ����� ������ ����.
* ������ ������ ������ ���� � ��������� ����� ������ ��� �� �������� ������, ����� �������� ����� ����� � ������� ������� ����. 
* ������ ������ ���� ��� �������� ����� ��� �������� ������ � ������� ������� ����. 



###����� ������
```
//������ ����� ������ �����������
List<Point> pixels = bezier.ToPixels;
//������ ����� ���������, �������������� �� ������ ������� GraphicsPath.Flatten.
List<Point> polyline = bezier.ToPolyline();
```

![Demo Screenshot](http://images.illuzor.com/uploads/bez-new.png)
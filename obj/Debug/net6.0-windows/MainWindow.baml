   M S B A M L   `   `   `  ����K  GFiles2Dicom_Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:Files2Dicom_TestFiles2Dicom_Test  5   	   !    Files2Dicom_Test.MainWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   0localclr-namespace:Files2Dicom_Test     5   	     � Title$  
MainWindow��5   	   $	��984��6   $	��800��6)   .��5	      � $��0,0,0,53q�6   .+�5
   
   �� -   5         ��Name$ btnStartScan��5
      $��StartFileScan��6'   $
��Left=�6?   $��	25,52,0,0q�6Z   $	��Top=�6m   $	��100��6�   �� 5   
   -   5         ��Name$ tbFileCount��5      $
��Left=�6'   $��
620,26,0,0q�6B    �� TextWrapping$
 Wrap=�6V   $��TextBox��6j   $	��Top=�6y   $	��120��6�   �� 5   
   -   5        $ tbStatus��5      $��Center=�6$   $��	0,103,0,0q�6A   $
 Wrap=�6T   $	��750��6h   $	��808��6t   $	��Top=�6�   �� 5   
   -   5        $ btnPauseScan��5      $��pause��6'   $
��Left=�67   $��
170,52,0,0q�6R   $	��Top=�6f   6~   �� 5   
   -   5        $ btnStopScan��5      $
��Stop��6&   $
��Left=�65   $��
232,52,0,0q�6P   $	��Top=�6d   6|   �� 5   
   $��	FileCount��6   $
��Left=�6$   $��
514,22,0,0q�6?   $	��Top=�6S   $	��106��6k   �� 5   
   -   5        $ tbNonDicomFiles��5      $
��Left=�6+   $��
620,48,0,0q�6F   $
 Wrap=�6Z   $��TextBox��6n   $	��Top=�6}   $	��120��6�   �� 5   
   $��NonDicomFiles��6   $
��Left=�6(   $��
514,44,0,0q�6C   $	��Top=�6W   $	��106��6o   �� 5   
   -   5        $ tbThreadCount��5      $
��Left=�6)   $��
620,75,0,0q�6D   $
 Wrap=�6X   $��Threads��6l   $	��Top=�6{   $	��120��6�   �� 5   
   $��Threads��6   $
��Left=�6"   $��
514,71,0,0q�6=   $	��Top=�6Q   $	��106��6i   5      5      
   M S B A M L   `   `   `  ����E  AClient GUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:Client_GUI
Client_GUI  5   	       Client_GUI.ServicesTest   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   *localclr-namespace:Client_GUI     5   	   -   5          � Title$  ServicesTest��5   	   $	��450��6   $	��800��6+   .��5
      � -   5         �Name$ MainDisplay��5
      .+�5   
   � -   5        $��ProgBar��5      $
��Left=�6%   $��18��6@   $��
10,391,0,0q�6L   $	��Top=�6`   $	��772��6x   �� 5   
   -   5        $��ServiceSearchBox��5      $
��Left=�6*   $��	10,12,0,0q�6E    �� TextWrapping$
 Wrap=�6X   $��Service Search��6l   $	��Top=�6�   $	��545��6�   �� 5   
   -   5        $��SearchServiceBtn��5      $��Search Service��6)   $
��Left=�6B   $��
568,11,0,0q�6]   $	��Top=�6q   $	��100��6�   6�   �� 5   
   -   5        $��AllServicesBtn��5      $��Find All Services��6'   $
��Left=�6C   $��
682,11,0,0q�6^   $	��Top=�6r   $	��100��6�   6�   �� 5   
   -   5        $��ServiceDropMenu��5       �� 
IsEditable .  �� 
IsReadOnly .  �� Text$# -- Please select a service --��6O   $
��Left=�6t   $��	10,45,0,0q�6�   $	��Top=�6�   $	��772��6�   6�   �� 5   
   	 �� X1$	 800��6   	 �� Y1$ 0��6   $��0,85,0,0q�6   
�������$��3��6?   �� 5   
   $
��Left=�6   $��
10,103,0,0q�6/   $
��Wrap=�6C   $��Name: ��6W   $	��Top=�6e   �� 5   
   $
��Left=�6   $��
10,133,0,0q�6/   $
��Wrap=�6C   $��Description: ��6W   $	��Top=�6l   �� 5   
   $
��Left=�6   $��
10,163,0,0q�6/   $
��Wrap=�6C   $��API Endpoint: ��6W   $	��Top=�6m   �� 5   
   $
��Left=�6   $��
10,193,0,0q�6/   $
��Wrap=�6C   $��Number of Operands: ��6W   $	��Top=�6s   �� 5   
   $
��Left=�6   $��
10,223,0,0q�6/   $
��Wrap=�6C   $��Operand Type: ��6W   $	��Top=�6m   �� 5   
   -   5        $��ServiceName��5      $
��Left=�6%   $��22��6@   $��133,103,0,0q�6L   $
 Wrap=�6a   $��Service Name��6u   $	��Top=�6�   $	��324��6�    �� 
IsReadOnly . 6�   �� 5   
   -	   5        $��ServiceDesc��5      $
��Left=�6%   $��22��6@   $��133,133,0,0q�6L   $
 Wrap=�6a   $��Service Description��6u   $	��Top=�6�   $	��324��6�    . 6�   �� 5   
   -
   5        $��ServicePoint��5      $
��Left=�6&   $��22��6A   $��133,163,0,0q�6M   $
 Wrap=�6b   $��Service API Endpoint��6v   $	��Top=�6�   $	��324��6�    . 6�   �� 5   
   -   5        $��ServiceNumOfOp��5      $
��Left=�6(   $��22��6C   $��133,193,0,0q�6O   $
 Wrap=�6d   $ ��Service Number of Operands��6x   $	��Top=�6�   $	��324��6�    . 6�   �� 5   
   -   5        $��ServiceOperandType��5      $
��Left=�6,   $��22��6G   $��133,223,0,0q�6S   $
 Wrap=�6h   $��Service Operand Type��6|   $	��Top=�6�   $	��324��6�    . 6�   �� 5   
   $ 0��6   $	 150��6   $��470,100,0,0q�6   
�������$��1��6B   �� 5   
   -   5        $��RunServiceBtn��5      $��Run Service��6&   $
��Left=�6<   $��480,261,0,0q�6W   $	��Top=�6l   $	��302��6�   $y�Hidden=�6�   �� 5   
   $
��Left=�6   $��
10,263,0,0q�6/   $
��Wrap=�6C   $��Service Output:��6W   $	��Top=�6n   �� 5   
   -   5        $��ServiceOutput��5      $
��Left=�6'   $	��100��6B   $��
10,286,0,0q�6O   $
 Wrap=�6c   $��Results��6w   $	��Top=�6�   $	��772��6�   5!      5"      
Варианты передачи параметров:
1) Через командную строку
2) Из файла конфигурации (1й способ не удался)
3) Если данные не получены первыми 2мя способами, то запрашиваем данные у пользователя

В командную строку могут быть переданные 3 варианта параметров:
1) 0 параметров - путь для Log-файла будет задан по умалчанию(в папке программой).
   Район, дата и время будут взяты из файла конфигурации или от пользователя
2) 1 параметр - путь для Log-файла будет взят из параметра командной строки
   Район, дата и время будут взяты из файла конфигурации или от пользователя
3) 3 параметра - путь для Log-файла, район, дата и время будут взяты из параметров командной строки
   (param1 Район, param2 Дата и время, param3 Log-файл) 
   либо, при некорекности данных, из файла конфигурации или от пользователя

SourceData.txt - файл с исходными данными.

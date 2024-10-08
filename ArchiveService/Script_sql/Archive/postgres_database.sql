CREATE TABLE IF NOT EXISTS public.parameters
(
    id_parameters integer  generated by default identity primary key,
    name_parameters varchar(30)
)
CREATE TABLE IF NOT EXISTS public.frequency
(
    id_frequency integer generated by default identity primary key,
    name_frequency  varchar(20)
)
CREATE TABLE IF NOT EXISTS public.archive
(
    id_archive integer generated by default identity primary key,
    id_frequency integer references public.frequency (id_frequency),
    id_parameters integer references public.parameters (id_parameters),
    chanel integer,
    datetime timestamp,
    mvkvalue real
)
INSERT INTO [dbo].[Frequency] ([Name]) VALUES ('10-2000гц')
INSERT INTO [dbo].[Frequency] ([Name]) VALUES ('10-1000гц')
INSERT INTO [dbo].[Frequency] ([Name]) VALUES ('2-1000гц')
INSERT INTO [dbo].[Frequency] ([Name]) VALUES ('25гц')
INSERT INTO [dbo].[Frequency] ([Name]) VALUES ('10-3000гц')
INSERT INTO [dbo].[Frequency] ([Name]) VALUES ('0.8-300гц')
INSERT INTO [dbo].[Frequency] ([Name]) VALUES ('0.8-150гц')
INSERT INTO [dbo].[Frequency] ([Name]) VALUES ('фильтр 1')
INSERT INTO [dbo].[Frequency] ([Name]) VALUES ('фильтр 2')

INSERT INTO [dbo].[Parameters] ([Name]) VALUES ('Пик-Фактор')
INSERT INTO [dbo].[Parameters] ([Name]) VALUES ('СКЗ виброускорение')
INSERT INTO [dbo].[Parameters] ([Name]) VALUES ('СКЗ виброскорость')
INSERT INTO [dbo].[Parameters] ([Name]) VALUES ('СКЗ виброперемещение')
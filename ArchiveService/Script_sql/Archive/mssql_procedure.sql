USE [MVK_Archive]
GO
/****** Object:  UserDefinedFunction [dbo].[GetFrequencyID]    Script Date: 27.02.2023 17:35:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create function [dbo].[GetFrequencyID](@name varchar(50))
returns int
as begin
return (select ID from [dbo].[Frequency] where Name like'%'+@name+'%')
end;
GO
/****** Object:  UserDefinedFunction [dbo].[GetParametersID]    Script Date: 27.02.2023 17:35:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create function [dbo].[GetParametersID](@name varchar(50))
returns int
as begin
return (select ID from [dbo].[Parameters] where Name like'%'+@name+'%')
end;
GO
/****** Object:  StoredProcedure [dbo].[AddValue]    Script Date: 27.02.2023 17:35:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[AddValue] @freq varchar(50),@param varchar(50), @chanel int, @data float
as begin
	insert Archive([Time],[ID Frequency],[ID Parameters],[Chanel],[MVK Value]) values (getdate(),[dbo].[GetFrequencyID](lower(@freq)),[dbo].[GetParametersID](lower(@param)),@chanel,@data)
end;
GO

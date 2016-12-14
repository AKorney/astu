/*
Created		11/17/2016
Modified		11/17/2016
Project		
Model			
Company		
Author		
Version		
Database		MS SQL 2005 
*/



















Create table [Account]
(
	[id_account] Int NOT NULL identity(1,1) primary key,
	[name] Nvarchar(300) NOT NULL       
) 
go

Create table [Transaction]
(
	[id_transaction] int NOT NULL identity(1,1) primary key,
	[id_type] Integer NOT NULL,
	[id_account] Integer NOT NULL,
	[id_category] Integer NOT NULL,
	[name] Nvarchar(300) NOT NULL,
	[date] Datetime NOT NULL,
	[notes] Nvarchar(300) NULL
)
 
go

Create table [Category]
(
	[id_category] Int NOT NULL identity(1,1) primary key,
	[id_type] Integer NOT NULL,
	[name] Nvarchar(300) NULL
) 
go

Create table [Type]
(
	[id_type] Int NOT NULL identity(1,1) primary key,
	[name] Nvarchar(300) NOT NULL
) 
go












Alter table [Transaction] add  foreign key([id_account]) references [Account] ([id_account])  on update cascade on delete cascade 
go
Alter table [Transaction] add  foreign key([id_category]) references [Category] ([id_type])  on update cascade on delete no action 
go
Alter table [Category] add  foreign key([id_type]) references [Type] ([id_type])  on update cascade on delete cascade 
go


Set quoted_identifier on
go











/* Update trigger "tu_Transaction" for table "Transaction" */
Create trigger [tu_Transaction]
on [Transaction] after update as
begin	 
		
	declare @numrows int 
	select @numrows = @@rowcount
	if @numrows = 0
		return
	
	
	/* Restrict parent "Account" when child "Transaction" updated */
 	if update([id_account])
 	begin
 		if ((select count(*)
 			from [Account] t, inserted i
 			where t.[id_account] = i.[id_account] ) != @numrows)
 			begin
 				throw  50002,'Parent does not exist in table ''Account''. Cannot update child table ''Transaction''.',1
 				rollback transaction
 				return
 			end
 	end		
 
 /* Restrict parent "Category" when child "Transaction" updated */
 	if update([id_category]) or
      update([id_type])
 	begin
 		if ((select count(*)
 			from [Category] t, inserted i
 			where t.[id_category] = i.[id_category] and 
          				t.[id_type] = i.[id_type] ) != @numrows)
 			begin
 				throw  50002, 'Parent does not exist in table ''Category''. Cannot update child table ''Transaction''.',1
 				rollback transaction
 				return
 			end
 	end		
 
 
	
end
go

/* Update trigger "tu_Category" for table "Category" */
Create trigger [tu_Category]
on [Category] after update as
begin	 
		
	declare @numrows int 
	select @numrows = @@rowcount
	if @numrows = 0
		return
	
	
	/* Restrict parent "Type" when child "Category" updated */
 	if update([id_type])
 	begin
 		if ((select count(*)
 			from [Type] t, inserted i
 			where t.[id_type] = i.[id_type] ) != @numrows)
 			begin
 				throw  50002,'Parent does not exist in table ''Type''. Cannot update child table ''Category''.',1
 				rollback transaction
 				return
 			end
 	end		
 
 
	
end
go



/* Insert trigger "ti_Transaction" for table "Transaction" */
Create trigger [ti_Transaction]
on [Transaction] after insert as
begin	 
	
	declare @numrows int
	select @numrows = @@rowcount
	if @numrows = 0
		return
	
		/* Restrict child "Transaction" when parent "Account" insert */
 	if update([id_account])
 	begin
 		if ((select count(*) 
 			from [Account] t, inserted i
 			where t.[id_account] = i.[id_account] ) != @numrows)
 			begin
 				throw  50004,'Parent does not exist in table ''Account''. Cannot insert into child table ''Transaction''.',1
 				rollback transaction
 				return
 			end
 	end		
 
 	/* Restrict child "Transaction" when parent "Category" insert */
 	if update([id_category]) or
      update([id_type])
 	begin
 		if ((select count(*) 
 			from [Category] t, inserted i
 			where t.[id_category] = i.[id_category] and 
          				t.[id_type] = i.[id_type] ) != @numrows)
 			begin
 				throw  50004,'Parent does not exist in table ''Category''. Cannot insert into child table ''Transaction''.',1
 				rollback transaction
 				return
 			end
 	end		
 
 
	
end
go

/* Insert trigger "ti_Category" for table "Category" */
Create trigger [ti_Category]
on [Category] after insert as
begin	 
	
	declare @numrows int
	select @numrows = @@rowcount
	if @numrows = 0
		return
	
		/* Restrict child "Category" when parent "Type" insert */
 	if update([id_type])
 	begin
 		if ((select count(*) 
 			from [Type] t, inserted i
 			where t.[id_type] = i.[id_type] ) != @numrows)
 			begin
 				throw  50004,'Parent does not exist in table ''Type''. Cannot insert into child table ''Category''.',1
 				rollback transaction
 				return
 			end
 	end		
 
 
	
end
go


Set quoted_identifier off
go






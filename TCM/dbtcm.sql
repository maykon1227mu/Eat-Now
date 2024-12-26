drop database dbtcm;
create database dbtcm;
use dbtcm;

create table tbusuario(
CodUsu int primary key auto_increment,
Nome varchar(80) not null,
Email varchar(40) not null,
Usuario varchar(40) not null,
Senha varchar(16) not null,
Tipo varchar(28) default "Cliente"
);

create table tbfuncionario(
CodFunc int primary key auto_increment,
Nome varchar(80) not null,
Email varchar(40) not null,
Usuario varchar(40) not null,
Senha varchar(16) not null,
Salario decimal(9,2) not null,
Tipo varchar(28) default "Funcionario"
);

create table tbfornecedor(
CodFor int primary key auto_increment,
Email varchar(40) not  null,
Usuario varchar(40) not null,
Senha varchar(16) not null,
CNPJ varchar(20) not null,
Tipo varchar(28) default "Fornecedor"
);

create table tbproduto(
CodProd int primary key auto_increment,
NomeProd varchar(40) not null,
Descricao varchar(80) not null,
Preco decimal(9,2) not null,
Qtd int unsigned not null,
UserId int not null,
Imagem mediumblob not null,
CategoriaId int not null
);

create table tbcategoria(
CodCat int primary key auto_increment,
Categoria varchar(40) not null
);

create table tbpedido(
CodPed int primary key auto_increment,
ProdutoId int not null,
UserId int not null,
QtdPed int unsigned not null,
DataPed datetime default current_timestamp
);

create table tbPromocao(
PromoId int primary key auto_increment,
NomePromo varchar(80) not null,
Porcentagem tinyint not null,
data_exclusao datetime not null
);

create table tbPromocaoItem(
PromoIdItem int primary key auto_increment,
ProdutoId int not null,
PrecoPromo decimal not null,
data_exclusao datetime not null
);

select * from tbusuario;

select tbpedido.CodPed, tbpedido.ProdutoId, tbpedido.UserId, tbpedido.DataPed, tbproduto.NomeProd, tbproduto.Preco, tbpedido.QtdPed from tbpedido join tbproduto on tbpedido.ProdutoId = tbproduto.CodProd where tbpedido.UserId = 1;

alter table tbpedido add constraint FK_ProdutoIdPedido foreign key (ProdutoId) references tbproduto(CodProd);
alter table tbpedido add constraint FK_UserIdPedido foreign key (UserId) references tbusuario(CodUsu);

create table tbcarrinho (
Id int primary key auto_increment,
UserId int not null,
ProdutoId int not null,
Quantidade int unsigned not null,
foreign key (ProdutoId) references tbproduto(CodProd)
);

delimiter $$
create procedure spInserirCarrinho(vUserId int, vProdutoId int , vQuantidade int)
begin
if(select Qtd from tbproduto where CodProd= vProdutoId != 0) then
	if not exists(select * from tbcarrinho where UserId = vUserId and ProdutoId = vProdutoId) then
		insert into tbcarrinho (UserId, ProdutoId, Quantidade) values (vUserId, vProdutoId, vQuantidade);
        update tbproduto set Qtd = Qtd - vQuantidade where CodProd = vProdutoId;
	else 
    update tbcarrinho set Quantidade = Quantidade + vQuantidade where UserId = vUserId and ProdutoId = vProdutoId;
    update tbproduto set Qtd = Qtd - vQuantidade where CodProd = vProdutoId;
    end if;
end if;
end
$$

delimiter ;

delimiter $$
create procedure spExcluirDoCarrinho(vUserId int, vProdutoId int , vQuantidade int)
begin
declare vQtd int;
set vQtd := (select Quantidade from tbcarrinho where UserId = vUserId and ProdutoId = vProdutoId);
if(vQtd <= 0 or vQtd = 1)then
	delete from tbcarrinho where UserId = vUserId and ProdutoId = vProdutoId;
	update tbproduto set Qtd = Qtd + vQuantidade where CodProd = vProdutoId;
    else
    update tbcarrinho set Quantidade = Quantidade - vQuantidade where UserId = vUserId and ProdutoId = vProdutoId;
    update tbproduto set Qtd = Qtd + vQuantidade where CodProd = vProdutoId;
    end if;
end
$$

delimiter ;

delimiter $$
create procedure spZerosCarrinho()
begin
	delete from tbcarrinho where Quantidade = 0;
end
$$

delimiter ;

delimiter $$
create procedure spLogin(vUsuario varchar(40), vSenha varchar(16))
begin
if exists(select * from tbusuario where usuario = vUsuario and senha = vSenha)then
	select * from tbusuario where usuario = vUsuario and senha = vSenha;
end if;
if exists(select * from tbfornecedor where usuario = vUsuario and senha = vSenha)then
	select * from tbfornecedor where usuario = vUsuario and senha = vSenha;
end if;
if exists(select * from tbfuncionario where usuario = vUsuario and senha = vSenha)then
	select * from tbfornecedor where usuario = vUsuario and senha = vSenha;
end if;
end;
$$

delimiter ;

call spInserirPromocao("natal", 50, "Comida Japonesa", '2024-12-26 10:58:00');

select * from tbpromocao;
select * from tbpromocaoitem;

DELIMITER $$

create PROCEDURE spInserirPromocao(vNomePromo VARCHAR(80), vPorcentagem int, vCategoria VARCHAR(80), vdata_exclusao DATETIME)
BEGIN
    

END$$

DELIMITER ;

alter table tbproduto add constraint FK_UserId_tbProduto foreign key (UserId) references tbfornecedor(CodFor);
alter table tbproduto add constraint FK_CategoriaId_tbProduto foreign key (CategoriaId) references tbcategoria(CodCat);

insert into tbcategoria (Categoria) values ("Comida Japonesa");
insert into tbcategoria (Categoria) values ("Comida Italiana");
insert into tbcategoria (Categoria) values ("Pizza");
insert into tbcategoria (Categoria) values ("Hamburguer");
insert into tbcategoria (Categoria) values ("Aperitivos");
insert into tbcategoria (Categoria) values ("Sorvete");
insert into tbcategoria (Categoria) values ("Milkshake");
insert into tbcategoria (Categoria) values ("Açai");
insert into tbcategoria (Categoria) values ("Bebidas");

insert into tbfornecedor (email, usuario, senha, CNPJ, tipo) values ( "admin@admin.com", "Admin", "12345", 1, "Administrador");
insert into tbfornecedor (email, usuario, senha, CNPJ) values ("fornecedorteste@gmail.com", "Fornecedor teste", "12345", "00.623.904/0001-73");
insert into tbfuncionario (Nome, email, usuario, senha, salario) values ("funcionarioteste", "funcionarioteste@gmail.com", "functeste", "12345", 1);
insert into tbusuario (Nome, email, usuario, senha) values ("Nathan", "nathanbs1227@gmail.com", "Nathanbsy", "12345");

select * from tbfornecedor;
select * from tbfuncionario;
select * from tbusuario;
select * from tbcarrinho;
select * from tbproduto;
select * from tbcategoria;
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
Imagem mediumblob not null
);

create table tbpedido(
CodPed int primary key auto_increment,
ProdutoId int not null,
UserId int not null,
QtdPed int unsigned not null,
DataPed datetime default current_timestamp
);

select * from tbusuario;

select tbpedido.CodPed, tbpedido.ProdutoId, tbpedido.UserId, tbpedido.DataPed, tbproduto.NomeProd, tbproduto.Preco, tbpedido.QtdPed from tbpedido join tbproduto on tbpedido.ProdutoId = tbproduto.CodProd where tbpedido.UserId = 1;

alter table tbpedido add constraint FK_ProdutoIdPedido foreign key (ProdutoId) references tbproduto(CodProd);
alter table tbpedido add constraint FK_UserIdPedido foreign key (UserId) references tbusuario(CodUsu);

CREATE TABLE tbcarrinho (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL,
    ProdutoId INT NOT NULL,
    Quantidade INT unsigned NOT NULL,
    FOREIGN KEY (ProdutoId) REFERENCES tbproduto(CodProd)
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

select * from tbcarrinho;
select * from tbproduto;

alter table tbproduto add constraint FK_UserId_tbProduto foreign key (UserId) references tbfornecedor(CodFor);

/*create table tbpedido(
CodPed int primary key auto_increment,
CodProd int,
NomePed varchar(40),
DescricaoPed varchar(80),
PrecoPed decimal(9,2),
QtdPed int
);

/*CREATE TABLE Produtos (
    ProdutoID INT PRIMARY KEY,
    Nome VARCHAR(100),
    Preco DECIMAL(10, 2)
);

CREATE TABLE Pedidos (
    PedidoID INT PRIMARY KEY,
    ProdutoID INT,
    Quantidade INT,
    FOREIGN KEY (ProdutoID) REFERENCES Produtos(ProdutoID)
);

SELECT Pedidos.PedidoID, Pedidos.Quantidade, Produtos.Nome, Produtos.Preco
FROM Pedidos
JOIN Produtos ON Pedidos.ProdutoID = Produtos.ProdutoID;

INSERT INTO Produtos (ProdutoID, Nome, Preco)
VALUES 
    (1, 'Camiseta', 29.90),
    (2, 'Calça', 79.90),
    (3, 'Tênis', 159.90),
    (4, 'Jaqueta', 129.90),
    (5, 'Boné', 19.90);
    
INSERT INTO Pedidos (PedidoID, ProdutoID, Quantidade)
VALUES 
    (1, 1, 2),  -- Pedido de 2 unidades da Camiseta
    (2, 3, 1),  -- Pedido de 1 unidade do Tênis
    (3, 2, 3),  -- Pedido de 3 unidades da Calça
    (4, 4, 1),  -- Pedido de 1 unidade da Jaqueta
    (5, 5, 4);  -- Pedido de 4 unidades do Boné
*/

insert into tbfornecedor (email, usuario, senha, cnpj, tipo) values ("admin@admin.com", "Admin", "12345", 1, "Administrador");
insert into tbusuario (nome, email, usuario, senha, salario, tipo) values ("Administrador", "admin@admin.com", "Admin", "12345", 1, "Administrador");
insert into tbfuncionario (nome, email, usuario, senha, tipo) values ("Administrador", "admin@admin.com", "Admin", "12345", "Administrador");

select * from tbfornecedor;
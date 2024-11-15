drop database dbtcm;
create database dbtcm;
use dbtcm;

create table tbusuario(
CodUsu int primary key auto_increment,
email varchar(40),
usuario varchar(40),
senha varchar(16),
tipo varchar(28) default "Cliente"
);

create table tbproduto(
CodProd int primary key auto_increment,
NomeProd varchar(40),
Descricao varchar(80),
Preco decimal(9,2),
Qtd int,
Imagem mediumblob
);


/*create table tbpedido(
CodPed int primary key auto_increment,
CodProd int,
NomePed varchar(40),
DescricaoPed varchar(80),
PrecoPed decimal(9,2),
QtdPed int
);

create table tbcarrinho(
CodCar int primary key auto_increment,
UserId int not null,
ProdutoId int,
QtdCar int
);*/

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

alter table tbpedido add constraint FK_NomePed foreign key (NomePed) references tbproduto(NomeProd);
alter table tbpedido add constraint FK_NomePed foreign key (CodProd) references tbproduto(CodProd);
alter table tbpedido add constraint FK_DescricaoPed foreign key (DescricaoPed) references tbproduto(Descricao);
alter table tbpedido add constraint FK_PrecoPed foreign key (PrecoPed) references tbproduto(Preco);
alter table tbpedido add constraint FK_QtdPed foreign key (QtdPed) references tbproduto(Qtd);
*/

insert into tbusuario (usuario, senha, tipo) values ("Admin", "12345", "Administrador");

update tbusuario set email = "admin@admin.com" where CodUsu = 1;

select * from tbproduto;
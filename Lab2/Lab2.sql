/*==============================================================*/
/* DBMS name:      MySQL 5.0                                    */
/* Created on:     2023/5/6 20:58:13                            */
/*==============================================================*/


drop table if exists Ascription;

drop table if exists Borrow;

drop table if exists Checking;

drop table if exists CheckingAccount;

drop table if exists Contacts;

drop table if exists Department;

drop table if exists LoanPay;

drop table if exists Person;

drop table if exists Response;

drop table if exists Saving;

drop table if exists SavingAccount;

drop table if exists Subbank;

/*==============================================================*/
/* Table: Ascription                                            */
/*==============================================================*/
create table Ascription
(
   ID                   char(20) not null,
   depID                char(20) not null,
   startDate            date not null,
   empType              bool not null,
   primary key (ID, depID)
);

/*==============================================================*/
/* Table: Borrow                                                */
/*==============================================================*/
create table Borrow
(
   loanID               char(20) not null,
   ID                   char(20) not null,
   bankName             char(20) not null,
   primary key (loanID, ID, bankName)
);

/*==============================================================*/
/* Table: Checking                                              */
/*==============================================================*/
create table Checking
(
   accID                char(20) not null,
   overdraft            float(8,2) not null,
   balance              float(8,2) not null,
   accDate              date not null,
   lastAccDate          date,
   primary key (accID)
);

/*==============================================================*/
/* Table: CheckingAccount                                       */
/*==============================================================*/
create table CheckingAccount
(
   ID                   char(20) not null,
   accID                char(20) not null,
   bankName             char(20) not null,
   primary key (ID, accID, bankName)
);

/*==============================================================*/
/* Table: Contacts                                              */
/*==============================================================*/
create table Contacts
(
   Per_ID               char(20) not null,
   ID                   char(20) not null,
   Relationship         char(20) not null,
   primary key (Per_ID, ID)
);

/*==============================================================*/
/* Table: Department                                            */
/*==============================================================*/
create table Department
(
   depID                char(20) not null,
   ID                   char(20) not null,
   depName              varchar(20) not null,
   primary key (depID)
);

/*==============================================================*/
/* Table: LoanPay                                               */
/*==============================================================*/
create table LoanPay
(
   loanID               char(20) not null,
   amount               float(8,2) not null,
   payDate              date not null,
   primary key (loanID)
);

/*==============================================================*/
/* Table: Person                                                */
/*==============================================================*/
create table Person
(
   ID                   char(20) not null,
   cusName              varchar(20) not null,
   phone                char(15) not null,
   address              varchar(50),
   Email                varchar(50),
   primary key (ID)
);

/*==============================================================*/
/* Table: Response                                              */
/*==============================================================*/
create table Response
(
   ID                   char(20) not null,
   Per_ID               char(20) not null,
   resType              char(20) not null,
   primary key (ID, Per_ID)
);

/*==============================================================*/
/* Table: Saving                                                */
/*==============================================================*/
create table Saving
(
   accID                char(20) not null,
   rate                 decimal not null,
   moneyType            char(20) not null,
   balance              float(8,2) not null,
   accDate              date not null,
   lastAccDate          date,
   primary key (accID)
);

/*==============================================================*/
/* Table: SavingAccount                                         */
/*==============================================================*/
create table SavingAccount
(
   ID                   char(20) not null,
   accID                char(20) not null,
   bankName             char(20) not null,
   primary key (ID, accID, bankName)
);

/*==============================================================*/
/* Table: Subbank                                               */
/*==============================================================*/
create table Subbank
(
   bankName             char(20) not null,
   asset                float(8,2) not null,
   primary key (bankName)
);

alter table Ascription add constraint FK_Ascription foreign key (ID)
      references Person (ID) on delete restrict on update restrict;

alter table Ascription add constraint FK_Ascription2 foreign key (depID)
      references Department (depID) on delete restrict on update restrict;

alter table Borrow add constraint FK_Borrow foreign key (loanID)
      references LoanPay (loanID) on delete restrict on update restrict;

alter table Borrow add constraint FK_Borrow2 foreign key (ID)
      references Person (ID) on delete restrict on update restrict;

alter table Borrow add constraint FK_Borrow3 foreign key (bankName)
      references Subbank (bankName) on delete restrict on update restrict;

alter table CheckingAccount add constraint FK_CheckingAccount foreign key (ID)
      references Person (ID) on delete restrict on update restrict;

alter table CheckingAccount add constraint FK_CheckingAccount2 foreign key (accID)
      references Checking (accID) on delete restrict on update restrict;

alter table CheckingAccount add constraint FK_CheckingAccount3 foreign key (bankName)
      references Subbank (bankName) on delete restrict on update restrict;

alter table Contacts add constraint FK_Contacts foreign key (Per_ID)
      references Person (ID) on delete restrict on update restrict;

alter table Contacts add constraint FK_Contacts2 foreign key (ID)
      references Person (ID) on delete restrict on update restrict;

alter table Department add constraint FK_Leaded foreign key (ID)
      references Person (ID) on delete restrict on update restrict;

alter table Response add constraint FK_Response foreign key (ID)
      references Person (ID) on delete restrict on update restrict;

alter table Response add constraint FK_Response2 foreign key (Per_ID)
      references Person (ID) on delete restrict on update restrict;

alter table SavingAccount add constraint FK_SavingAccount foreign key (ID)
      references Person (ID) on delete restrict on update restrict;

alter table SavingAccount add constraint FK_SavingAccount2 foreign key (accID)
      references Saving (accID) on delete restrict on update restrict;

alter table SavingAccount add constraint FK_SavingAccount3 foreign key (bankName)
      references Subbank (bankName) on delete restrict on update restrict;


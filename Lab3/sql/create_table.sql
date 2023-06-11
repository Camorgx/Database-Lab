/*==============================================================*/
/* DBMS name:      MySQL 5.0                                    */
/* Created on:     2023/6/5 15:47:33                            */
/*==============================================================*/


drop table if exists Teach;

drop table if exists TeacherAccount;

drop table if exists Lesson;

drop table if exists Publish;

drop table if exists Undertake;

drop table if exists Paper;

drop table if exists Project;

drop table if exists Teacher;

/*==============================================================*/
/* Table: Lesson                                                */
/*==============================================================*/
create table Lesson
(
   lessonID             char(255) not null,
   lessonName           char(255),
   lessonHour           int,
   lessonType           int,
   primary key (lessonID)
);

/*==============================================================*/
/* Table: Paper                                                 */
/*==============================================================*/
create table Paper
(
   paperID              int not null,
   paperName            char(255),
   paperSource          char(255),
   paperYear            int,
   paperType            int,
   level                int,
   primary key (paperID)
);

/*==============================================================*/
/* Table: Project                                               */
/*==============================================================*/
create table Project
(
   projectID            char(255) not null,
   projectName          char(255),
   projectSource        char(255),
   projectType          int,
   totalMoney           float,
   startYear            int,
   endYear              int,
   primary key (projectID)
);

/*==============================================================*/
/* Table: Publish                                               */
/*==============================================================*/
create table Publish
(
   teacherID            char(5) not null,
   paperID              int not null,
   paperRank            int,
   corresponding        bool,
   primary key (teacherID, paperID)
);

/*==============================================================*/
/* Table: Teach                                                 */
/*==============================================================*/
create table Teach
(
   teacherID            char(5) not null,
   lessonID             char(255) not null,
   year                 int,
   term                 int,
   hour                 int,
   primary key (teacherID, lessonID, year, term)
);

/*==============================================================*/
/* Table: Teacher                                               */
/*==============================================================*/
create table Teacher
(
   teacherID            char(5) not null,
   teacherName          char(255),
   gender               int,
   title                int,
   primary key (teacherID)
);

/*==============================================================*/
/* Table: Undertake                                             */
/*==============================================================*/
create table Undertake
(
   teacherID            char(5) not null,
   projectID            char(255) not null,
   projectRank          int,
   money                float,
   primary key (teacherID, projectID)
);

alter table Publish add constraint FK_Publish foreign key (teacherID)
      references Teacher (teacherID) on delete restrict on update restrict;

alter table Publish add constraint FK_Publish2 foreign key (paperID)
      references Paper (paperID) on delete restrict on update restrict;

alter table Teach add constraint FK_Teach foreign key (teacherID)
      references Teacher (teacherID) on delete restrict on update restrict;

alter table Teach add constraint FK_Teach2 foreign key (lessonID)
      references Lesson (lessonID) on delete restrict on update restrict;

alter table Undertake add constraint FK_Undertake foreign key (teacherID)
      references Teacher (teacherID) on delete restrict on update restrict;

alter table Undertake add constraint FK_Undertake2 foreign key (projectID)
      references Project (projectID) on delete restrict on update restrict;

create table TeacherAccount
(
    teacherID            char(5) primary key,
    password             varchar(20) not null,
    verification         varchar(20) not null
);

alter table TeacherAccount add constraint FK_TeacherAccount foreign key (teacherID)
      references Teacher (teacherID) on delete restrict on update restrict;


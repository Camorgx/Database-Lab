import random

project = []
undertake = []
lesson = []
teach = []
publish = []

for i in range(1, 21):
    projectID = '%05d' % i
    projectName = f'Project{i}'
    projectCandidate = random.randint(1, 5)
    totalMoney = 0
    teacherList = [k for k in range(1, 21)]
    random.shuffle(teacherList)
    for j in range(1, projectCandidate + 1):
        teacher = teacherList[j - 1]
        rank = j
        money = random.randint(1, 10)
        undertake.append("('%05d', '%s', %d, %d)" % (teacher, projectID, rank, money))
        totalMoney += money
    project.append("('%s', '%s', 'ProjectSource%d', %d, %d, %d, %d)" %
                   (projectID, projectName, i, random.randint(1, 5), totalMoney,
                    random.randint(2000, 2010), random.randint(2010, 2020)))

    lessonID = '%05d' % i
    lessonName = f'Lesson{i}'
    termCnt = random.randint(1, 3)
    totalHour = 0
    year = random.randint(2010, 2020)
    random.shuffle(teacherList)
    for j in range(1, termCnt + 1):
        teacher = teacherList[j - 1]
        term = j
        hour = random.randint(1, 10)
        teach.append("('%05d', '%s', %d, %d, %d)" % (teacher, lessonID, year, term, hour))
        totalHour += hour
    lesson.append("('%s', '%s', %d, %d)" % (lessonID, lessonName, totalHour, random.randint(1, 2)))

    paperID = i
    paperCandidate = random.randint(1, 5)
    correspondingID = random.randint(1, paperCandidate)
    random.shuffle(teacherList)
    for j in range(1, paperCandidate + 1):
        teacher = teacherList[j - 1]
        publish.append("('%05d', %d, %d, %s)" % (teacher, paperID, j, j == correspondingID))


with open('insert_data.sql', 'w') as file:
    for i in range(1, 21):
        file.write("insert into Teacher value ('%05d', 'Teacher%d', %d, %d);\n"
                   % (i, i, random.randint(1, 2), random.randint(1, 11)))
    file.write('\n')
    for i in range(1, 21):
        file.write("insert into TeacherAccount value ('%05d', 'abc%05d', 'verification%d');\n" % (i, i, i))
    file.write('\n')
    for i in range(1, 21):
        file.write("insert into Paper value (%d, 'Paper%d', 'PaperSource%d', %d, %d, %d);\n"
                   % (i, i, i, random.randint(2010, 2020), random.randint(1, 4), random.randint(1, 6)))
    file.write('\n')
    for item in publish:
        file.write(f"insert into Publish value {item};\n")
    file.write('\n')
    for item in lesson:
        file.write(f"insert into Lesson value {item};\n")
    file.write('\n')
    for item in teach:
        file.write(f"insert into Teach value {item};\n")
    file.write('\n')
    for item in project:
        file.write(f"insert into Project value {item};\n")
    file.write('\n')
    for item in undertake:
        file.write(f"insert into Undertake value {item};\n")
    file.write('\n')

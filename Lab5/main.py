inp = open("graphedges.txt","r")
k = list(map(int,inp.readline().split()))

nodes = []
for i in range(1000):
    nodes.append(list())

for i in range(0,len(k),2):
    a1 = k[i]
    a2 = k[i+1]
    nodes[a1].append(a2)
    nodes[a2].append(a1)

for i in range(1000):
    nodes[i] = list(set(nodes[i]))

isolates = []
edgesCount = 0
maxNodeNums = []
maxNodeSte = 0
for i in range(1000):
    if len(nodes[i])==0:
        isolates.append(i)
    else:
        edgesCount+=len(nodes[i])
        if len(nodes[i])>maxNodeSte:
            maxNodeNums = [i]
            maxNodeSte = len(nodes[i])
        elif len(nodes[i])==maxNodeSte:
            maxNodeNums.append(i)

edgesCount = int(edgesCount/2)

print("Граней -",edgesCount)
print("Изолятов -",len(isolates))
print(*isolates)
print("Наибольшая степень вершин -", maxNodeSte)
for i in maxNodeNums:
    print(i,"-",*nodes[i])


used = [False]*1000

subgraphs = []

def dfs(v,n):
    used[v] = True
    subgraphs[n].append(v)
    for i in nodes[v]:
        if not used[i]:
            dfs(i,n)

for i in range(1000):
    if not used[i]:
        subgraphs.append([])
        dfs(i,len(subgraphs)-1)

used = [False]*1000

nowNodes = sorted(subgraphs,key=lambda x:-len(x))[0]

levels = [-1]*len(nodes)
ways = []
for i in range(1000):
    ways.append([])
def bfs(s):
    levels[s] = 0
    queue = [s]
    ways[s] = [s]
    while queue:
        v = queue.pop(0)
        for w in nodes[v]:
            if levels[w] == -1:
                queue.append(w)
                levels[w] = levels[v]+1
                ways[w] = ways[v].copy()
                ways[w].append(w)

for i in range(1000):
    if levels[i]==-1:
        bfs(i)

ind = levels.index(max(levels))

levels = [-1]*1000

bfs(ind)

print("Диаметр -",max(levels))

find = [[78,560],[445,830],[359,104]]

for i in find:
    levels = [-1]*1000
    bfs(i[0])
    print("Путь между",i[0],"и",i[1],"-",levels[i[1]])
    print(*ways[i[1]])

toDelete = [706, 100, 266, 523, 592, 659]

for i in range(0,1000,17):
    toDelete.append(i)
for i in range(1000):
    if i in toDelete:
        nodes[i] = ["DELETED"]
    else:
        for j in toDelete:
            if j in nodes[i]:
                nodes[i].remove(j)
print("После удаления:")
isolates = []
edgesCount = 0
maxNodeNums = []
maxNodeSte = 0
for i in range(1000):
    if len(nodes[i])==0:
        isolates.append(i)
    else:
        if (nodes[i][0]=="DELETED"):
            nodes[i] = []
            continue
        edgesCount+=len(nodes[i])
        if len(nodes[i])>maxNodeSte:
            maxNodeNums = [i]
            maxNodeSte = len(nodes[i])
        elif len(nodes[i])==maxNodeSte:
            maxNodeNums.append(i)

edgesCount = int(edgesCount/2)

print("Граней -",edgesCount)
print("Изолятов -",len(isolates))
print(*isolates)
print("Наибольшая степень вершин -", maxNodeSte)
for i in maxNodeNums:
    print(i,"-",*nodes[i])

used = [False]*1000

subgraphs = []

for i in range(1000):
    if not used[i]:
        subgraphs.append([])
        dfs(i,len(subgraphs)-1)

used = [False]*1000

nowNodes = sorted(subgraphs,key=lambda x:-len(x))[0]

levels = [-1]*len(nodes)
ways = []
for i in range(1000):
    ways.append([])

for i in range(1000):
    if levels[i]==-1:
        bfs(i)

ind = levels.index(max(levels))

levels = [-1]*1000

bfs(ind)

print("Диаметр -",max(levels))

for i in find:
    levels = [-1]*1000
    bfs(i[0])
    print("Путь между",i[0],"и",i[1],"-",levels[i[1]])
    print(*ways[i[1]])

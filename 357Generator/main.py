import random

# 参数设置
filePath = './output.csv'

round = input("请输入回合数：")
# 写入标题
with open(filePath, 'a') as output_file:
    output_file.write("B1,B2,B3,SelectBlock,Count,IsWinner\n")

for i in range(1, int(round) + 1):
    print("回合", i)
    dashboard = [3, 5, 7]
    roundList = []  # 先定义空集合
    player = 1  # 玩家1操作(切换后)
    # 回合循环
    while dashboard[0] + dashboard[1] + dashboard[2] > 0:
        selectBlock = random.randint(0, 2)  # 选择的方块(0,1,2)
        if dashboard[selectBlock] == 0:
            continue  # 如果选中的方块无可操作项，则跳到下一个循环
        else:
            player = 1-player  # 切换玩家

            # 选择数量
            count = random.randint(1, dashboard[selectBlock])
            roundList.append([dashboard[0], dashboard[1],
                             dashboard[2], selectBlock, count, player])
            dashboard[selectBlock] -= count

    # 回合循环完毕，此时player为输家
    listLength = len(roundList)  # 列表长度

    # for debug:
    # print("listLength:", listLength)
    # print(roundList)

    with open(filePath, 'a') as output_file:
        for i in range(0, listLength):
            roundList[i][5] = 0 if roundList[i][5] == player else 1 # 将玩家代号转为输赢代号

            output_file.write("%s,%s,%s,%s,%s,%s\n" % (
                roundList[i][0], roundList[i][1], roundList[i][2], roundList[i][3], roundList[i][4], roundList[i][5]))
            
            # for debug
            # print(roundList[i])
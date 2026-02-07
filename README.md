# TimedAirdrop

Unturned Rocket 插件，允许 VIP 玩家召唤空投。

## 功能

- `/airdrop` 命令召唤空投到地图随机位置
- VIP 玩家冷却时间 12 小时，SVIP 玩家 4 小时
- 冷却时间持久化，服务器重启后保留

## 权限

| 权限 | 说明 |
|------|------|
| `timedairdrop.vip` | VIP，12小时冷却 |
| `timedairdrop.svip` | SVIP，4小时冷却 |

## 安装

1. 编译项目：`dotnet build`
2. 复制 `bin/Debug/net48/TimedAirdrop.dll` 到服务器 `Rocket/Plugins` 目录

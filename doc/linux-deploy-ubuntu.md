chroot部署的Linux并不能使用一些过于底层的功能。因为它只是在（监狱）中运行的发行

下面解答一些由于安卓特性导致的问题：

如果你在这上面运行mysql 或者 PostgreSQL 等其他使用套接字的软件时

那么会遇到没有套接字权限的问题

在安卓上跑需要多一个步骤：

将运行用户加入套接字权限组：

mysql:

sudo usermod -G aid_inet mysql
postgresql:

sudo usermod -G aid_inet postgres
普通运行socket的用户：

sudo usermod -G aid_inet username

add 
Include /etc/phpmyadmin/apache.conf
to
/etc/apache2/apache2.conf
echo 'Include /etc/phpmyadmin/apache.conf' | sudo tee -a /etc/apache2/apache2.conf

sudo apt install php-mbstring php-gettext

sudo apt installl -y libmysqlclient-dev

sudo gem install 'mysql'
sudo gem install mysql2 -v 0.4.0
sudo service apache2 restart

echo "gem 'mysql'"|sudo tee -a /usr/share/redmine/Gemfile
sudo bundle exec rails server webrick -e production -b 0.0.0.0
import tornado.httpserver
import tornado.ioloop
import tornado.web
import pymongo
import datetime
import bson

db = pymongo.Connection().posts


class Posts(tornado.web.RequestHandler):

    def get(self):
        self.render("posts.html", posts=db.posts.find().sort("date",
            direction=pymongo.DESCENDING))


class NewPost(tornado.web.RequestHandler):

    def post(self):
        db.posts.insert({"text": self.get_argument("textpost"),
            "date": datetime.datetime.now(),
            "comments": [],
            "likes": 0}, safe=True)
        self.redirect("/")


class NewComment(tornado.web.RequestHandler):

    def post(self):
        db.posts.update({"_id": bson.ObjectId(self.get_argument("idpost"))},
            {"$push": {"comments": {"text": self.get_argument("comment", ""),
                "date": datetime.datetime.now()}}})
        self.redirect("/")


class NewLike(tornado.web.RequestHandler):

    def post(self):
        db.posts.update({"_id": bson.ObjectId(self.get_argument("idpost"))},
            {"$inc": {"likes": 1}})
        self.redirect("/")


settings = {
    'template_path': "templates",
    'debug': False
    }

application = tornado.web.Application([
    (r"/", Posts),
    (r"/newpost", NewPost),
    (r"/newlike", NewLike),
    (r"/newcomment", NewComment),
        ], **settings)


def run():
    http_server = tornado.httpserver.HTTPServer(application)
    http_server.listen(8081)
    tornado.ioloop.IOLoop.instance().start()

if __name__ == '__main__':
    run()

require 'rubygems'
require 'sinatra'
require 'mongo'

db = Mongo::Connection.new['products']
products = db['products']

get '/' do
  @PRODUCTS = products.find()
  erb :products
end

post '/newproduct' do
  product = {"name" => params[:name],
	"description" => params[:description],
	"price" => params[:price],
	"quantity" => params[:quantity],
	"votes" => 0,
	"rating" => 0}
  products.insert(product)
  redirect '/'
end

post '/rate' do
  product = products.find_one({"_id" => BSON::ObjectId(params[:idproduct])})
  rating = 0
  if product["votes"] == 0
    rating = Integer(params[:vote])
  else
    rating = (product["rating"] + Integer(params[:vote]))/2
  end
  products.update({"_id" => BSON::ObjectId(params[:idproduct])},
    {"$inc" => {"votes" => 1},
      "$set" => {"rating" => rating}})
  redirect '/'
end

import os
import openai

openai.api_key = os.getenv("OPENAI_API_KEY")

response = openai.Completion.create(
  model="text-davinci-003",
  prompt="What 3 products do I need to make my hair shiny and smooth?\n\n1. Shampoo and Conditioner: Look for a shampoo and conditioner formulated with ingredients that help hydrate and nourish your hair, such as argan oil, coconut oil, shea butter, and jojoba oil. \n\n2. Hair Mask: A nourishing hair mask can help lock in moisture and restore shine, softness, and smoothness to your hair. Products containing avocado oil, keratin, and vitamin E are great for this purpose. \n\n3. Heat-Protective Serum: Heat styling can damage your hair, so it’s important to use a heat-protective serum before using any hot tools. This will help to keep your hair shiny and smooth.",
  temperature=0.7,
  max_tokens=3654,
  top_p=1,
  frequency_penalty=0,
  presence_penalty=0
)